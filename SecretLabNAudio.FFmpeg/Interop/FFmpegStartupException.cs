namespace SecretLabNAudio.FFmpeg.Interop;

public sealed class FFmpegStartupException : Exception
{

    public static FFmpegStartupException Last => new(FFmpegSL.LastCaughtStartError);

    public NativeErrorCode Error { get; }

    public FFmpegStartupException(NativeErrorCode error) => Error = error;

}
