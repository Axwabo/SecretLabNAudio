using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Providers;
using SecretLabNAudio.FFmpeg.Interop;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    [ThreadStatic]
    private static MemoryStream? _memoryStream;

    private static RawSourceSampleProvider? Read(FFmpegSL? ffmpeg, TimeSpan? maxDuration)
    {
        if (ffmpeg == null)
            return null;
        _memoryStream ??= new MemoryStream();
        _memoryStream.Position = 0;
        ffmpeg.Stdout.BaseStream.CopyTo(_memoryStream);
        ffmpeg.WaitForExit(1000);
        var byteSpan = _memoryStream.GetBuffer().AsSpan(0, (int) _memoryStream.Position);
        var floatSpan = MemoryMarshal.Cast<byte, float>(byteSpan);
        if (maxDuration.HasValue && floatSpan.Length > maxDuration.Value.TotalSeconds * AudioPlayer.SampleRate)
            return null;
        ffmpeg.ThrowIfExitedWithError();
        return new RawSourceSampleProvider(floatSpan.ToArray(), AudioPlayer.SupportedFormat);
    }

    public static RawSourceSampleProvider? ReadWithFFmpeg(string input, TimeSpan? maxDuration = null)
    {
        using var process = FFmpegSL.PlayerCompatibleToStdout(input);
        return Read(process, maxDuration);
    }

    public static RawSourceSampleProvider? ReadWithFFmpeg(FFmpegArguments arguments, TimeSpan? maxDuration = null)
    {
        using var process = FFmpegSL.StartRaw(arguments.ForPlayerCompatibleFloatPiping());
        return Read(process, maxDuration);
    }

}
