namespace SecretLabNAudio.Core.FileReading;

/// <summary>Quick methods to create <see cref="StreamAudioProcessor"/>s based on the file type.</summary>
/// <remarks>This class does not protect against nonexistent files.</remarks>
public static class CreateAudioProcessor
{

    private static StreamAudioProcessor Convert(this AudioReaderFactoryResult result, string type, string? path = null) => result switch
    {
        ({ } stream, { } provider) => new StreamAudioProcessor(stream, provider) {FilePath = path},
        ({ } stream, null) => new StreamAudioProcessor(stream) {FilePath = path},
        _ => throw new NotSupportedException($"Factory for {type} did not return a WaveStream")
    };

    /// <summary>
    /// Creates a <see cref="StreamAudioProcessor"/> from a file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>An audio processor that reads from the file.</returns>
    /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
    /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
    public static StreamAudioProcessor FromFile(string path)
    {
        var type = Path.GetExtension(path);
        return AudioReaderFactoryManager.GetFactory(type).FromPath(path).Convert(type, path);
    }

    /// <summary>
    /// Creates a <see cref="StreamAudioProcessor"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="baseStream">The stream to read from.</param>
    /// <param name="fileType">The type of the audio file.</param>
    /// <param name="isOwned">Whether to dispose of <paramref name="baseStream"/> when the processor is disposed.</param>
    /// <returns>An audio processor that reads from the stream.</returns>
    /// <include file='../XmlDocs/Files.xml' path='doc/exception[@name="NotSupported"]'/>
    public static StreamAudioProcessor FromStream(Stream baseStream, string fileType, bool isOwned = true)
        => AudioReaderFactoryManager.GetFactory(fileType).FromStream(baseStream, isOwned).Convert(fileType);

}
