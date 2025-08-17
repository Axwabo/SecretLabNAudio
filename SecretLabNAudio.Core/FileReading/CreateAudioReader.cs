using System.IO;
using StreamAndProvider = (NAudio.Wave.WaveStream Stream, NAudio.Wave.ISampleProvider Provider);

namespace SecretLabNAudio.Core.FileReading;

/// <summary>Methods for creating <see cref="WaveStream"/>s and <see cref="ISampleProvider"/>.</summary>
/// <remarks>This class does not protect against nonexistent files.</remarks>
public static class CreateAudioReader
{

    private static AudioReaderFactoryResult Result(string path, string type)
        => AudioReaderFactoryManager.GetFactory(type).FromPath(path);

    private static AudioReaderFactoryResult Result(Stream source, string type, bool closeOnDispose)
        => AudioReaderFactoryManager.GetFactory(type).FromStream(source, closeOnDispose);

    private static WaveStream GetStream(this AudioReaderFactoryResult result, string fileType)
        => result.Stream ?? throw new NotSupportedException($"Factory for {fileType} did not return a WaveStream");

    private static ISampleProvider GetProvider(this AudioReaderFactoryResult result, string fileType, bool convertStream) => result switch
    {
        (_, { } provider) => provider,
        ({ } stream, _) when convertStream => stream.ToSampleProvider(),
        _ => throw new NotSupportedException($"Factory for {fileType} did not return a SampleProvider")
    };

    /// <summary>
    /// Creates a <see cref="WaveStream"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <returns>A <see cref="WaveStream"/> corresponding to the file.</returns>
    /// <exception cref="NotSupportedException">Thrown if there was no registered factory for the file type, or if the factory didn't return a <see cref="WaveStream"/>.</exception>
    /// <remarks>This method doesn't check if the file exists. Call <see cref="File.Exists">File.Exists</see> beforehand.</remarks>
    public static WaveStream Stream(string path)
    {
        var type = Path.GetExtension(path);
        return Result(path, type).GetStream(type);
    }

    /// <summary>
    /// Creates a <see cref="WaveStream"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="source">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the <see cref="WaveStream"/>.</param>
    /// <returns>A <see cref="WaveStream"/> corresponding to the stream.</returns>
    /// <remarks>The period is automatically trimmed from the start of the <paramref name="fileType"/>.</remarks>
    public static WaveStream Stream(Stream source, string fileType, bool closeOnDispose = true)
        => Result(source, fileType, closeOnDispose).GetStream(fileType);

    /// <summary>
    /// Creates an <see cref="ISampleProvider"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="convertStream">Whether to convert the <see cref="WaveStream"/> to an <see cref="ISampleProvider"/> if the factory only returns a <see cref="WaveStream"/>.</param>
    /// <returns>An <see cref="ISampleProvider"/> corresponding to the stream.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if there was no registered factory for the file type,
    /// or if the factory didn't return an <see cref="ISampleProvider"/> and <paramref name="convertStream"/> is false.
    /// </exception>
    /// <remarks>
    /// This method discards the <see cref="WaveStream"/> returned by the factory. The caller is responsible for disposing the <paramref name="stream"/>.
    /// The period is automatically trimmed from the start of the <paramref name="fileType"/>.
    /// </remarks>
    public static ISampleProvider Provider(Stream stream, string fileType, bool convertStream = true)
        => Result(stream, fileType, false).GetProvider(fileType, convertStream);

    /// <summary>
    /// Creates a <see cref="WaveStream"/> and its converted <see cref="ISampleProvider"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <returns>A tuple containing a <see cref="WaveStream"/> and its corresponding <see cref="ISampleProvider"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if no factory was registered for the file type, or if the factory didn't return both a <see cref="WaveStream"/> and an <see cref="ISampleProvider"/>.</exception>
    /// <remarks>This method doesn't check if the file exists. Call <see cref="File.Exists">File.Exists</see> beforehand.</remarks>
    public static StreamAndProvider StreamAndProvider(string path)
    {
        var type = Path.GetExtension(path);
        return Result(path, type) is ({ } stream, { } provider)
            ? (stream, provider)
            : throw new NotSupportedException($"Factory for {type} did not return both a stream and a provider");
    }

    /// <summary>
    /// Creates a <see cref="WaveStream"/> and its converted <see cref="ISampleProvider"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="source">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the <see cref="WaveStream"/>.</param>
    /// <returns>A tuple containing a <see cref="WaveStream"/> and its corresponding <see cref="ISampleProvider"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if no factory was registered for the file type, or if the factory didn't return both a <see cref="WaveStream"/> and an <see cref="ISampleProvider"/>.</exception>
    /// <remarks>The period is automatically trimmed from the start of the <paramref name="fileType"/>.</remarks>
    public static StreamAndProvider StreamAndProvider(Stream source, string fileType, bool closeOnDispose = true)
        => Result(source, fileType, closeOnDispose) is ({ } stream, { } provider)
            ? (stream, provider)
            : throw new NotSupportedException($"Factory for {fileType} did not return both a stream and a provider");

}
