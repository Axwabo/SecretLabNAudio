using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.FFmpeg.Interop;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class SynchronousFFmpegAudioProcessor : IAudioProcessor
{

    private readonly FFmpegSL _ffmpeg;

    public static SynchronousFFmpegAudioProcessor CreatePlayerCompatible(string path) => new(
        FFmpegSL.StartRaw(FFmpegArgumentsBuilder.PlayerCompatibleToStdout(path)) ?? throw FFmpegStartException.Last,
        AudioPlayer.SupportedFormat
    );

    public static SynchronousFFmpegAudioProcessor Create(string path, int sampleRate, int channels) => new(
        FFmpegSL.StartRaw(FFmpegArgumentsBuilder.ToStdout(path, sampleRate, channels)) ?? throw FFmpegStartException.Last,
        WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels)
    );

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

    public void Dispose() => _ffmpeg.Dispose();

}
