namespace SecretLabNAudio.Core.FileReading;

/// <summary>Methods for creating <see cref="StreamAudioProcessor"/>s with the try pattern.</summary>
/// <remarks>This class does not protect against nonexistent files.</remarks>
public static class TryCreateAudioProcessor
{

    private static bool TryCreate(string type, Func<IAudioReaderFactory, AudioReaderFactoryResult> create, [NotNullWhen(true)] out StreamAudioProcessor? processor)
    {
        if (!AudioReaderFactoryManager.TryGetFactory(type, out var factory))
        {
            processor = null;
            return false;
        }

        processor = create(factory) switch
        {
            ({ } stream, { } provider) => new StreamAudioProcessor(stream, provider),
            ({ } stream, _) => new StreamAudioProcessor(stream),
            _ => null
        };
        return processor != null;
    }

    /// <summary>
    /// Attempts to create a <see cref="StreamAudioProcessor"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <param name="processor">
    /// The resulting <see cref="StreamAudioProcessor"/> if successful.
    /// <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.
    /// </param>
    /// <returns>Whether a <see cref="StreamAudioProcessor"/> was successfully created.</returns>
    /// <include file='../XmlDocs/Files.xml' path='doc/exception[@name="NotFound"]'/>
    /// <remarks>
    /// This method doesn't check if the file exists. Call <see cref="File.Exists">File.Exists</see> beforehand.
    /// The underlying file stream is automatically disposed when the processor is disposed.
    /// </remarks>
    public static bool FromFile(string path, [NotNullWhen(true)] out StreamAudioProcessor? processor)
        => TryCreate(Path.GetExtension(path), factory => factory.FromPath(path), out processor);

    /// <summary>
    /// Attempts to create a <see cref="StreamAudioProcessor"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="baseStream">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="isOwned">Whether to close the stream when disposing the <see cref="StreamAudioProcessor"/>.</param>
    /// <param name="processor">
    /// The resulting <see cref="StreamAudioProcessor"/> if successful.
    /// <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.
    /// </param>
    /// <returns>Whether a <see cref="StreamAudioProcessor"/> was successfully created.</returns>
    /// <remarks>
    /// The period is automatically trimmed from the start of the <paramref name="fileType"/>.
    /// The underlying file stream is automatically disposed when the processor is disposed.
    /// </remarks>
    public static bool FromStream(Stream baseStream, string fileType, bool isOwned, [NotNullWhen(true)] out StreamAudioProcessor? processor)
        => TryCreate(Path.GetExtension(fileType), factory => factory.FromStream(baseStream, isOwned), out processor);

}
