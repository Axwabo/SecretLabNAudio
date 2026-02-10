using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg;

public sealed class SynchronousFFmpegAudioProcessor : IAudioProcessor
{

    private readonly FFmpegSL _ffmpeg;

    public static SynchronousFFmpegAudioProcessor Create(string path)
    {
        var process = FFmpegSL.StartRaw($"-i \"{path}\" -ar 48000 -ac 1 -f f32le -");
        return process == null ? throw new InvalidOperationException("watafak bro") : new SynchronousFFmpegAudioProcessor(process);
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
