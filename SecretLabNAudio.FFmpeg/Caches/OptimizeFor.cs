namespace SecretLabNAudio.FFmpeg.Caches;

/// <summary>
/// Represents values indicating a cached audio file's type.
/// </summary>
public enum OptimizeFor
{

    /// <summary>
    /// Optimize for reading performance (WAV).
    /// </summary>
    ReadingSpeed,

    /// <summary>
    /// Optimize for a smaller file size (Ogg Vorbis).
    /// </summary>
    FileSize

}
