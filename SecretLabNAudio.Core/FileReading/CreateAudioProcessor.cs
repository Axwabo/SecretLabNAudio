namespace SecretLabNAudio.Core.FileReading;

/// <summary>Quick methods to create <see cref="WaveStreamAudioProcessor"/>s based on the file type.</summary>
/// <remarks>This class does not protect against nonexistent files.</remarks>
public static class CreateAudioProcessor
{

    /// <summary>
    /// Creates a <see cref="WaveStreamAudioProcessor"/> from a file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>An audio processor that reads from the file.</returns>
    /// <include file="../XmlDocs/Files.xml" path="doc/exception"/>
    /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
    public static IStreamAudioProcessor FromFile(string path)
        => AudioReaderFactoryManager.GetFactory(Path.GetExtension(path)).FromPath(path)
           ?? throw new NotSupportedException(); // TODO

    /// <summary>
    /// Creates a <see cref="WaveStreamAudioProcessor"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="baseStream">The stream to read from.</param>
    /// <param name="fileType">The type of the audio file.</param>
    /// <param name="isOwned">Whether to dispose of <paramref name="baseStream"/> when the processor is disposed.</param>
    /// <returns>An audio processor that reads from the stream.</returns>
    /// <include file="../XmlDocs/Files.xml" path="doc/exception[@name='NotSupported']"/>
    public static IStreamAudioProcessor FromStream(Stream baseStream, string fileType, bool isOwned = true)
        => AudioReaderFactoryManager.GetFactory(fileType).FromStream(baseStream, isOwned)
           ?? throw new NotSupportedException(); // TODO

}
