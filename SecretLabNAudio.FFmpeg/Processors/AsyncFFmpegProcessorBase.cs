using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NAudio.Utils;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg.Processors;

/// <summary>
/// A base FFmpeg audio processor that buffers the standard output asynchronously.
/// This class cannot be inherited from user code.
/// </summary>
/// <seealso cref="AsyncBufferedFFmpegAudioProcessor"/>
/// <seealso cref="StreamBasedFFmpegAudioProcessor"/>
public abstract class AsyncFFmpegProcessorBase : IAudioProcessor, IFFmpegWrapper
{

    private const TaskCreationOptions Options = TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning;

    private const int BufferSize = AudioPlayer.SamplesPerPacket * sizeof(float);

    /// <summary>
    /// The minimum capacity of the underlying buffer, measured in samples.
    /// </summary>
    public const int MinCapacitySamples = BufferSize;

    /// <summary>
    /// The default capacity of the underlying buffer in seconds.
    /// </summary>
    public const double DefaultCapacity = 10;

    [ThreadStatic]
    private static byte[]? _readBuffer;

    private readonly CircularBuffer _buffer;

    private protected readonly CancellationToken Token;

    private CancellationTokenSource? _cts;

    private protected FFmpegSL? Process { get; private set; }

    /// <summary>
    /// The state of the buffering thread.
    /// </summary>
    public AsyncBufferingState BufferingState { get; protected set; }

    /// <summary>
    /// The error (if any) that was encountered during the startup of FFmpeg.
    /// </summary>
    public NativeErrorCode StartupError { get; private set; }

    /// <summary>
    /// The exception (if any) that occurred during async buffering.
    /// </summary>
    public Exception? AsyncException { get; protected set; }

    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// The capacity of the buffer in samples.
    /// </summary>
    public int BufferCapacitySamples => _buffer.MaxLength;

    /// <summary>
    /// If the buffer contains more than this many samples, the buffering loop will wait for 100ms before checking again.
    /// </summary>
    public int SleepThresholdSamples
    {
        get;
        set => field = Mathf.Clamp(value, 0, BufferCapacitySamples);
    }

    /// <summary>
    /// If the buffer contains more samples than the equivalent of this value based on the <see cref="WaveFormat"/>, the buffering loop will wait for 100ms before checking again.
    /// </summary>
    public double SleepThresholdSeconds
    {
        get => WaveFormat.Seconds(SleepThresholdSamples);
        set => SleepThresholdSamples = WaveFormat.SampleCount(value);
    }

    /// <summary>
    /// If true, the buffering thread will always try to write to the buffer. If not enough data is available, <see cref="Read"/> will pad the data with zeroes.
    /// </summary>
    /// <seealso cref="StopBuffering"/>
    public bool Endless { get; set; }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public bool HasExited => Process?.HasExited ?? false;

    /// <inheritdoc/>
    public int ExitCode => Process?.ExitCode ?? throw new InvalidOperationException("The process has not started yet.");

    /// <inheritdoc/>
    public string? FinalErrorMessage => Process?.FinalErrorMessage;

    private protected AsyncFFmpegProcessorBase(double capacity, WaveFormat format)
    {
        WaveFormat = format;
        _buffer = new CircularBuffer(Mathf.Max(MinCapacitySamples, format.SampleCount(capacity) * sizeof(float)));
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
        ffmpeg = Process = FFmpegSL.Start(arguments, true);
        if (IsDisposed)
        {
            ffmpeg?.Dispose();
            return false;
        }

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
            if (read == 0 && !Endless)
                break;
            _buffer.Write(buffer, 0, read - read % sizeof(float));
        }

        BufferingState = AsyncBufferingState.Ended;
    }

    /// <inheritdoc/>
    /// <remarks>The target <paramref name="buffer"/> is filled with zeroes until reading has begun (unless if an error was encountered).</remarks>
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
        if (read >= count || !Endless || BufferingState != AsyncBufferingState.Reading || AsyncException != null)
            return floatSpan.Length;
        destination[floatSpan.Length..].Clear();
        BufferingState = AsyncBufferingState.PreFillingBuffer;
        return count;
    }

    /// <summary>
    /// Stops the buffering thread. The remaining data in the buffer will still be readable.
    /// </summary>
    /// <seealso cref="ClearBuffer"/>
    public virtual void StopBuffering()
    {
        BufferingState = AsyncBufferingState.Ended;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    /// <summary>
    /// Resets the buffer to 0 samples. The buffering thread is not guaranteed to stop.
    /// </summary>
    /// <param name="waitForRefill">
    /// If true, sets the state to <see cref="AsyncBufferingState.PreFillingBuffer"/>,
    /// padding this provider with silence until the buffer has at least <see cref="SleepThresholdSamples"/>.
    /// </param>
    /// <seealso cref="StopBuffering"/>
    public void ClearBuffer(bool waitForRefill)
    {
        if (BufferingState == AsyncBufferingState.Reading && waitForRefill)
            BufferingState = AsyncBufferingState.PreFillingBuffer;
        _buffer.Reset();
    }

    /// <inheritdoc cref="FFmpegSL.Dispose"/>
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        StopBuffering();
        Process?.Dispose();
    }

}
