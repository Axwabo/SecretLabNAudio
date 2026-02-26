namespace SecretLabNAudio.FFmpeg.Processors;

public enum AsyncBufferingState
{

    ResolvingStream,
    StartingFFmpeg,
    PreFillingBuffer,
    Reading,
    Ended

}
