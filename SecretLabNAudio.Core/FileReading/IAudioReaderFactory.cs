namespace SecretLabNAudio.Core.FileReading;

/// <summary>An interface for creating <see cref="WaveStream"/>s, given a file path or a stream.</summary>
public interface IAudioReaderFactory
{

    /// <summary>
    /// Creates a <see cref="StreamAudioProcessor"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <returns>A processor on success, null otherwise.</returns>
    StreamAudioProcessor FromPath(string path);

    /// <summary>
    /// Creates a <see cref="StreamAudioProcessor"/> from the given <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to read the audio from.</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the processor.</param>
    /// <returns>A processor on success, null otherwise.</returns>
    StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose);

}
