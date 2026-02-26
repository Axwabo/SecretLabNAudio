using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NAudio.Utils;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class AsyncBufferedFFmpegAudioProcessor : IAudioProcessor
{

    private const TaskCreationOptions Options = TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning;

    private const int BufferSize = AudioPlayer.SamplesPerPacket * sizeof(float);

    [ThreadStatic]
    private static byte[]? _readBuffer;

    private bool _disposed;

    private FFmpegSL? _process;

    private readonly CircularBuffer _buffer;

    private CancellationTokenSource? _cts = new();

    private readonly bool _isStdin;

    public NativeErrorCode StartupError { get; private set; }

    public AsyncBufferingState BufferingState { get; private set; }

    public Exception? AsyncException { get; private set; }

    public WaveFormat WaveFormat { get; }

    public string? FinalErrorMessage => _process?.FinalErrorMessage;

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format)
    {
        _buffer = new CircularBuffer(format.SampleCount(capacity * sizeof(float)));
        WaveFormat = format;
        BufferingState = AsyncBufferingState.StartingFFmpeg;
        Run(async token =>
        {
            var ffmpeg = _process = FFmpegSL.ToStdout(input, WaveFormat);
            if (ffmpeg == null)
                StartupError = FFmpegSL.LastCaughtStartError;
            else
                await BufferLoop(ffmpeg, token);
        });
    }

    private AsyncBufferedFFmpegAudioProcessor(Func<Awaitable<Stream>> inputPipeResolver, double capacitySeconds, int sampleRate, int channels)
    {
        _isStdin = true;
        _buffer = new CircularBuffer((int) (sampleRate * channels * (capacitySeconds * sizeof(float))));
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        _ = Startup(inputPipeResolver);
        return;

        async Awaitable Startup(Func<Awaitable<Stream>> resolver)
        {
            await Awaitable.BackgroundThreadAsync();
            var stream = await resolver();
            var ffmpeg = _process = FFmpegSL.ToStdout(FFmpegArguments.StandardPipe, WaveFormat);
            if (ffmpeg == null)
            {
                StartupError = FFmpegSL.LastCaughtStartError;
                stream.Dispose();
                return;
            }

            Run(token => stream.CopyToAsync(ffmpeg.Stdin!.BaseStream, token), stream);
            Run(token => BufferLoop(ffmpeg, token));
        }
    }

    private void Run(Func<CancellationToken, Task> action, IDisposable? disposable = null) => Task.Factory.StartNew(async () =>
    {
        var token = _cts!.Token;
        try
        {
            await action(token);
        }
        catch (OperationCanceledException)
        {
            BufferingState = AsyncBufferingState.Ended;
        }
        catch (Exception e)
        {
            AsyncException = e;
        }
        finally
        {
            disposable?.Dispose();
        }
    }, CancellationToken.None, Options, TaskScheduler.Default);

    private async Task BufferLoop(FFmpegSL ffmpeg, CancellationToken token)
    {
        BufferingState = AsyncBufferingState.PreFillingBuffer;
        while (!token.IsCancellationRequested)
        {
            if (_buffer.Count > _buffer.MaxLength * 0.9)
            {
                if (BufferingState == AsyncBufferingState.PreFillingBuffer)
                    BufferingState = AsyncBufferingState.Reading;
                await Task.Delay(100, token);
                continue;
            }

            // this is so cooked (thread static)
            var buffer = _readBuffer = BufferHelpers.Ensure(_readBuffer, BufferSize);
            var read = await ffmpeg.Stdout.BaseStream.ReadAsync(_readBuffer, 0, _readBuffer.Length, token);
            if (read == 0)
                break;
            _buffer.Write(buffer, 0, read);
        }

        BufferingState = AsyncBufferingState.Ended;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (StartupError == NativeErrorCode.None && BufferingState is not (AsyncBufferingState.Reading or AsyncBufferingState.Ended))
        {
            buffer.AsSpan(offset, count).Clear();
            return count;
        }

        var bytes = count * sizeof(float);
        _readBuffer = BufferHelpers.Ensure(_readBuffer, bytes);
        var read = _buffer.Read(_readBuffer, 0, bytes);
        var readSpan = _readBuffer.AsSpan(0, read);
        var floatSpan = MemoryMarshal.Cast<byte, float>(readSpan);
        floatSpan.CopyTo(buffer.AsSpan(offset, count));
        return floatSpan.Length;
    }

    public void StopBuffering()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        if (!_disposed && !_isStdin && _process is {HasExited: false})
            _process.TryTerminateGracefully(0);
        BufferingState = AsyncBufferingState.Ended;
    }

    public void ClearBuffer(bool refill)
    {
        if (BufferingState == AsyncBufferingState.Reading && refill)
            BufferingState = AsyncBufferingState.PreFillingBuffer;
        _buffer.Reset();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        StopBuffering();
        _process?.Dispose();
        _process = null;
    }

}
