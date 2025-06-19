using System.IO;

namespace SecretLabNAudio.Core.FileReading;

/// <summary>An interface for creating <see cref="WaveStream"/>s, given a file path or a stream.</summary>
public interface IAudioReaderFactory
{

    /// <summary>
    /// Creates an <see cref="AudioReaderFactoryResult"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <returns>The factory result.</returns>
    AudioReaderFactoryResult FromPath(string path);

    /// <summary>
    /// Creates an <see cref="AudioReaderFactoryResult"/> from the given <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to read the audio from.</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the <see cref="WaveStream"/>.</param>
    /// <returns>The factory result.</returns>
    AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose);

}
