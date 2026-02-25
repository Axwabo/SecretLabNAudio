using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.FFmpeg.Interop;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class SynchronousFFmpegAudioProcessor : IAudioProcessor
{

    private readonly FFmpegSL _ffmpeg;

    public static SynchronousFFmpegAudioProcessor Create(string path)
    {
        var process = FFmpegSL.StartRaw(FFmpegArgumentsBuilder.PlayerCompatible.WithInput(path));
        return new SynchronousFFmpegAudioProcessor(process ?? throw FFmpegStartException.Last);
    }

    private SynchronousFFmpegAudioProcessor(FFmpegSL ffmpeg) => _ffmpeg = ffmpeg;

    public int Read(float[] buffer, int offset, int count)
    {
        var floatSpan = buffer.AsSpan(offset, count);
        var byteSpan = MemoryMarshal.Cast<float, byte>(floatSpan);
        return _ffmpeg.Stdout.BaseStream.Read(byteSpan) / sizeof(float);
    }

    public WaveFormat WaveFormat => AudioPlayer.SupportedFormat;

    public void Dispose() => _ffmpeg.Dispose();

}
