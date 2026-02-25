using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;
using SecretLabNAudio.FFmpeg.Interop;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class ShortClipCacheExtensions
{

    [ThreadStatic]
    private static MemoryStream? _memoryStream;

    public static RawSourceSampleProvider? ReadWithFFmpeg(string input, TimeSpan? maxDuration)
    {
        using var process = FFmpegSL.PlayerCompatibleToStdout(input);
        if (process == null)
            return null;
        _memoryStream ??= new MemoryStream();
        _memoryStream.Position = 0;
        process.Stdout.BaseStream.CopyTo(_memoryStream);
        var buffer = _memoryStream.GetBuffer();
        var byteSpan = buffer.AsSpan(0, (int) _memoryStream.Position);
        var floatSpan = MemoryMarshal.Cast<byte, float>(byteSpan);
        if (maxDuration.HasValue && floatSpan.Length > maxDuration.Value.TotalSeconds * AudioPlayer.SampleRate)
            return null;
        process.WaitForExit();
        var error = process.Stdout.ReadToEnd();
        if (string.IsNullOrWhiteSpace(error))
            return new RawSourceSampleProvider(floatSpan.ToArray(), AudioPlayer.SupportedFormat);
        Debug.Log(error);
        return null;
    }

    extension(ShortClipCache)
    {

        public static RawSourceSampleProvider? AddWithFFmpeg(string input, TimeSpan? maxDuration = null) => ReadWithFFmpeg(input, maxDuration);

    }

}
