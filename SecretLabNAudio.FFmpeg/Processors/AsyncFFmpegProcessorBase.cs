using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NAudio.Utils;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg.Processors;

public abstract class AsyncFFmpegProcessorBase : IAudioProcessor
{

    private const TaskCreationOptions Options = TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning;

    private const int BufferSize = AudioPlayer.SamplesPerPacket * sizeof(float);

    public const double DefaultCapacity = 10;

    [ThreadStatic]
    private static byte[]? _readBuffer;

    private readonly CircularBuffer _buffer;

    private protected readonly CancellationToken Token;

    private CancellationTokenSource? _cts;

    private protected FFmpegSL? Process { get; private set; }

    public AsyncBufferingState BufferingState { get; protected set; }

    public NativeErrorCode StartupError { get; protected set; }

    public Exception? AsyncException { get; protected set; }

    public bool Disposed { get; private set; }

    public WaveFormat WaveFormat { get; }

    public string? FinalErrorMessage => Process?.FinalErrorMessage;

    public int BufferCapacitySamples => _buffer.MaxLength;

    public int SleepThresholdSamples
    {
        get;
        set => field = value < 0
            ? throw new ArgumentOutOfRangeException(nameof(value), "Sleep threshold samples must not be negative")
            : value > BufferCapacitySamples
                ? throw new ArgumentOutOfRangeException(nameof(value), "Sleep threshold samples must not be greater than the buffer's capacity")
                : value;
    }

    public double SleepThresholdSeconds
    {
        get => WaveFormat.Seconds(SleepThresholdSamples);
        set => SleepThresholdSamples = WaveFormat.SampleCount(value);
    }

    public bool AutoRefill { get; set; }

    private protected AsyncFFmpegProcessorBase(double capacity, WaveFormat format)
    {
        WaveFormat = format;
        _buffer = new CircularBuffer(format.SampleCount(capacity) * sizeof(float));
        _cts = new CancellationTokenSource();
        Token = _cts.Token;
        SleepThresholdSeconds = capacity * 0.75;
    }

    private protected void Offload(Action action) => Task.Factory.StartNew(() =>
    {
        try
        {
            action();
        }
        catch (Exception e) when (!Token.IsCancellationRequested)
        {
            AsyncException = e;
        }
    }, CancellationToken.None, Options, TaskScheduler.Default);

    private protected bool TryStartFFmpeg(string arguments, [NotNullWhen(true)] out FFmpegSL? ffmpeg)
    {
        ffmpeg = Process = FFmpegSL.StartRaw(arguments, true);
        if (ffmpeg != null)
            return true;
        StartupError = FFmpegSL.LastCaughtStartError;
        return false;
    }

    private protected void BufferLoop(FFmpegSL ffmpeg)
    {
        BufferingState = AsyncBufferingState.PreFillingBuffer;
        var buffer = _readBuffer = BufferHelpers.Ensure(_readBuffer, BufferSize);
        while (!Token.IsCancellationRequested)
        {
            if (_buffer.Count > SleepThresholdSamples)
            {
                BufferingState = AsyncBufferingState.Reading;
                Thread.Sleep(100);
                continue;
            }

            var read = ffmpeg.Stdout.BaseStream.Read(buffer, 0, buffer.Length);
            if (read == 0)
                break;
            _buffer.Write(buffer, 0, read - read % sizeof(float));
        }

        BufferingState = AsyncBufferingState.Ended;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var destination = buffer.AsSpan(offset, count);
        if (StartupError == NativeErrorCode.None && AsyncException == null && BufferingState is not (AsyncBufferingState.Reading or AsyncBufferingState.Ended))
        {
            destination.Clear();
            return count;
        }

        var bytes = count * sizeof(float);
        _readBuffer = BufferHelpers.Ensure(_readBuffer, bytes);
        var read = _buffer.Read(_readBuffer, 0, bytes);
        var readSpan = _readBuffer.AsSpan(0, read);
        var floatSpan = MemoryMarshal.Cast<byte, float>(readSpan);
        floatSpan.CopyTo(destination);
        if (read >= count || !AutoRefill || BufferingState != AsyncBufferingState.Reading || AsyncException != null)
            return floatSpan.Length;
        destination[floatSpan.Length..].Clear();
        BufferingState = AsyncBufferingState.PreFillingBuffer;
        return count;
    }

    public virtual void StopBuffering()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void ClearBuffer(bool waitForRefill)
    {
        if (BufferingState == AsyncBufferingState.Reading && waitForRefill)
            BufferingState = AsyncBufferingState.PreFillingBuffer;
        _buffer.Reset();
    }

    public void Dispose()
    {
        if (Disposed)
            return;
        Disposed = true;
        StopBuffering();
        Process?.Dispose();
    }

}
