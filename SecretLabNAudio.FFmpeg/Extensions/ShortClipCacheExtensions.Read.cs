using System.Runtime.InteropServices;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    [ThreadStatic]
    private static MemoryStream? _memoryStream;

    private static RawSourceSampleProvider? Read(FFmpegSL? ffmpeg, TimeSpan? maxDuration, string input)
    {
        if (ffmpeg == null)
            return null;
        _memoryStream ??= new MemoryStream();
        _memoryStream.Position = 0;
        ffmpeg.Stdout.BaseStream.CopyTo(_memoryStream);
        ffmpeg.WaitForExit();
        var byteSpan = _memoryStream.GetBuffer().AsSpan(0, (int) _memoryStream.Position);
        var floatSpan = MemoryMarshal.Cast<byte, float>(byteSpan);
        if (maxDuration.HasValue && floatSpan.Length > maxDuration.Value.TotalSeconds * AudioPlayer.SampleRate)
            return null;
        if (!ffmpeg.HasExitedWithError)
            return new RawSourceSampleProvider(floatSpan.ToArray(), AudioPlayer.SupportedFormat);
        Debug.LogError($"FFmpeg exited with code {ffmpeg.ExitCode} while reading player-compatible clip from {input}");
        if (ErrorMessageAnalyzer.HasNoOutputStream(ffmpeg.FinalErrorMessage))
            Debug.LogError("The input most likely does not contain audio");
        Debug.LogError(ffmpeg.FinalErrorMessage);
        return null;
    }

    /// <summary>
    /// Attempts to read an input source with FFmpeg as <see cref="AudioPlayer.SupportedFormat">player-compatible</see> samples.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <param name="maxDuration">If not null and a file's duration is longer than this value, the samples will be discarded.</param>
    /// <returns>
    /// A player-compatible <see cref="RawSourceSampleProvider" /> if the file was successfully read.
    /// Null if the file doesn't exist, if it couldn't be read, or if the duration exceeds <paramref name="maxDuration"/>.
    /// </returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
    public static RawSourceSampleProvider? ReadWithFFmpeg(string input, TimeSpan? maxDuration = null)
    {
        using var process = FFmpegSL.PlayerCompatibleToStdout(input);
        return Read(process, maxDuration, input);
    }

    /// <summary>
    /// Attempts to read an input source with FFmpeg as <see cref="AudioPlayer.SupportedFormat">player-compatible</see> samples.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="maxDuration">If not null and a file's duration is longer than this value, the samples will be discarded.</param>
    /// <returns>
    /// A player-compatible <see cref="RawSourceSampleProvider" /> if the file was successfully read.
    /// Null if the file doesn't exist, if it couldn't be read, or if the duration exceeds <paramref name="maxDuration"/>.
    /// </returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
    public static RawSourceSampleProvider? ReadWithFFmpeg(FFmpegArguments arguments, TimeSpan? maxDuration = null)
    {
        using var process = FFmpegSL.Start(arguments.ForPlayerCompatibleFloatPiping());
        return Read(process, maxDuration, arguments.Input!);
    }

}
