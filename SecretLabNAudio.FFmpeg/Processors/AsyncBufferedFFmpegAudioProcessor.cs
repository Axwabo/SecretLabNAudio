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

    private CancellationTokenSource? _cts;

    private readonly CancellationToken _token;

    private readonly bool _isStdin;

    public AsyncBufferingState BufferingState { get; private set; }

    public NativeErrorCode StartupError { get; private set; }

    public Exception? AsyncException { get; private set; }

    public WaveFormat WaveFormat { get; }

    private AsyncBufferedFFmpegAudioProcessor(double capacity, WaveFormat format)
    {
        WaveFormat = format;
        _buffer = new CircularBuffer(format.SampleCount(capacity) * sizeof(float));
        _cts = new CancellationTokenSource();
        _token = _cts.Token;
    }

    public AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : this(capacity, format) => Run(() =>
    {
        var ffmpeg = _process = FFmpegSL.ToStdout(input, WaveFormat);
        if (ffmpeg == null)
            StartupError = FFmpegSL.LastCaughtStartError;
        else
            BufferLoop(ffmpeg);
    });

    public AsyncBufferedFFmpegAudioProcessor(Func<Awaitable<Stream>> inputPipeResolver, double capacity, int sampleRate, int channels)
        : this(capacity, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels))
    {
        _isStdin = true;
        _buffer = new CircularBuffer((int) (sampleRate * channels * (capacity * sizeof(float))));
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        _ = StartAsync(inputPipeResolver);
    }

    private void Run(Action action) => Task.Factory.StartNew(() =>
    {
        try
        {
            action();
        }
        catch (Exception e) when (!_token.IsCancellationRequested)
        {
            AsyncException = e;
        }
    }, CancellationToken.None, Options, TaskScheduler.Default);

    private void BufferLoop(FFmpegSL ffmpeg)
    {
        BufferingState = AsyncBufferingState.PreFillingBuffer;
        while (!_token.IsCancellationRequested)
        {
            if (_buffer.Count > _buffer.MaxLength * 0.75)
            {
                BufferingState = AsyncBufferingState.Reading;
                Thread.Sleep(100);
                continue;
            }

            _readBuffer = BufferHelpers.Ensure(_readBuffer, BufferSize);
            var read = ffmpeg.Stdout.BaseStream.Read(_readBuffer, 0, _readBuffer.Length);
            if (read == 0)
                break;
            _buffer.Write(_readBuffer, 0, read);
        }

        BufferingState = AsyncBufferingState.Ended;
    }

    private async Awaitable StartAsync(Func<Awaitable<Stream>> resolver)
    {
        await Awaitable.BackgroundThreadAsync();
        try
        {
            await using var stream = await resolver();
            var ffmpeg = _process = FFmpegSL.ToStdout(FFmpegArguments.StandardPipe, WaveFormat);
            if (ffmpeg == null)
            {
                StartupError = FFmpegSL.LastCaughtStartError;
                return;
            }

            Run(() => BufferLoop(ffmpeg));
            await stream.CopyToAsync(ffmpeg.Stdin!.BaseStream, _token);
        }
        catch (Exception e) when (!_token.IsCancellationRequested)
        {
            AsyncException = e;
        }
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (StartupError == NativeErrorCode.None && AsyncException == null && BufferingState is not (AsyncBufferingState.Reading or AsyncBufferingState.Ended))
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
