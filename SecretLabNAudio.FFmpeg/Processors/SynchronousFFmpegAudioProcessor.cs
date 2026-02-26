using System.Runtime.InteropServices;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class SynchronousFFmpegAudioProcessor : IAudioProcessor
{

    private readonly FFmpegSL _ffmpeg;

    private SynchronousFFmpegAudioProcessor(FFmpegSL ffmpeg, WaveFormat format)
    {
        _ffmpeg = ffmpeg;
        WaveFormat = format;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var floatSpan = buffer.AsSpan(offset, count);
        var byteSpan = MemoryMarshal.Cast<float, byte>(floatSpan);
        return _ffmpeg.Stdout.BaseStream.Read(byteSpan) / sizeof(float);
    }

    public WaveFormat WaveFormat { get; }

    public bool HasExited => _ffmpeg.HasExited;

    public string? FinalErrorMessage => _ffmpeg.FinalErrorMessage;

    public void Dispose() => _ffmpeg.Dispose();

}
