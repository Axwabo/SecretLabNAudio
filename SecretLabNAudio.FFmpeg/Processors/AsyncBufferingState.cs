namespace SecretLabNAudio.FFmpeg.Processors;

public enum AsyncBufferingState
{

    StartingFFmpeg,
    ResolvingStream,
    PreFillingBuffer,
    Reading,
    Ended

}
