namespace SecretLabNAudio.FFmpeg.Interop;

public sealed class FFmpegStartException : Exception
{

    public static FFmpegStartException Last => new(FFmpegSL.LastCaughtStartError);

    public NativeErrorCode Error { get; }

    public FFmpegStartException(NativeErrorCode error) => Error = error;

}
