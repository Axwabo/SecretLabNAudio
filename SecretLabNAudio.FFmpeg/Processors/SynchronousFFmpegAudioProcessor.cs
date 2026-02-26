using System.Runtime.InteropServices;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class SynchronousFFmpegAudioProcessor : IAudioProcessor
{

    private readonly FFmpegSL _ffmpeg;

    public static SynchronousFFmpegAudioProcessor CreatePlayerCompatible(string path) => new(
        FFmpegSL.PlayerCompatibleToStdout(path) ?? throw FFmpegStartException.Last,
        AudioPlayer.SupportedFormat
    );

    public static SynchronousFFmpegAudioProcessor Create(string path, int sampleRate, int channels) => new(
        FFmpegSL.ToStdout(path, sampleRate, channels) ?? throw FFmpegStartException.Last,
        WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels)
    );

    public static SynchronousFFmpegAudioProcessor Create(FFmpegArguments arguments)
    {
        if (arguments.SampleRate <= 0 || arguments.Channels is not (1 or 2))
            throw new ArgumentException("Invalid wave format");
        var ffmpeg = FFmpegSL.StartRaw(arguments.ForFloatPiping()) ?? throw FFmpegStartException.Last;
        return new SynchronousFFmpegAudioProcessor(ffmpeg, WaveFormat.CreateIeeeFloatWaveFormat(arguments.SampleRate, arguments.Channels));
    }

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
