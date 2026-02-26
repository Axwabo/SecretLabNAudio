namespace SecretLabNAudio.FFmpeg.Processors;

public enum AsyncBufferingState
{

    WaitingForStreamResolver,
    StartingFFmpeg,
    PreFillingBuffer,
    Reading,
    Ended

}
