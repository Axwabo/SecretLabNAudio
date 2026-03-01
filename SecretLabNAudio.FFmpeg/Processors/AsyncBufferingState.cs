namespace SecretLabNAudio.FFmpeg.Processors;

/// <summary>
/// Represents states an <see cref="AsyncFFmpegProcessorBase"/>'s buffering thread can be in. 
/// </summary>
public enum AsyncBufferingState
{

    /// <summary>Resolving the stream to read from.</summary>
    ResolvingStream,

    /// <summary>Waiting for the FFmpeg process to start.</summary>
    StartingFFmpeg,

    /// <summary>Waiting for enough data before outputting samples from the buffer.</summary>
    PreFillingBuffer,

    /// <summary>Reading data from FFmpeg while the buffer's data is readable.</summary>
    Reading,

    /// <summary>The async buffering thread has exited. The remaining data in the buffer is still readable.</summary>
    Ended

}
