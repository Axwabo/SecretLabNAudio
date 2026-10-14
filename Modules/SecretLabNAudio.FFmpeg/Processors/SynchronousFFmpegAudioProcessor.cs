using System.Runtime.InteropServices;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg.Processors;

/// <summary>
/// A fully synchronous FFmpeg-based audio processor.
/// The static constructor methods and <see cref="Read"/> block until FFmpeg starts or outputs enough data, respectively. 
/// </summary>
public sealed partial class SynchronousFFmpegAudioProcessor : IAudioProcessor, IFFmpegWrapper
{

    private readonly FFmpegSL _ffmpeg;

    private SynchronousFFmpegAudioProcessor(FFmpegSL ffmpeg, WaveFormat format)
    {
        _ffmpeg = ffmpeg;
        WaveFormat = format;
    }

    /// <summary>
    /// Reads 32-bit float samples from FFmpeg.
    /// Blocks the current thread until enough data is read.
    /// </summary>
    /// <param name="buffer">The buffer to fill with samples.</param>
    /// <param name="offset">Offset into buffer</param>
    /// <param name="count">The number of samples to read</param>
    /// <returns>the number of samples written to the buffer.</returns>
    public int Read(float[] buffer, int offset, int count)
    {
        var floatSpan = buffer.AsSpan(offset, count);
        var byteSpan = MemoryMarshal.Cast<float, byte>(floatSpan);
        return _ffmpeg.Stdout.BaseStream.Read(byteSpan) / sizeof(float);
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public bool HasExited => _ffmpeg.HasExited;

    /// <inheritdoc/>
    public int ExitCode => _ffmpeg.ExitCode;

    /// <inheritdoc/>
    public string? FinalErrorMessage => _ffmpeg.FinalErrorMessage;

    /// <inheritdoc/>
    public bool IsDisposed => _ffmpeg.IsDisposed;

    /// <inheritdoc cref="FFmpegSL.Dispose" />
    public void Dispose() => _ffmpeg.Dispose();

}
