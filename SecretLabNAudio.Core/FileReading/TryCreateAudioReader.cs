using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SecretLabNAudio.Core.FileReading;

/// <summary>Non-throwing methods for creating <see cref="WaveStream"/>s and <see cref="ISampleProvider"/>.</summary>
public static class TryCreateAudioReader
{

    private static AudioReaderFactoryResult GetResult(string type, Func<IAudioReaderFactory, AudioReaderFactoryResult> create)
        => AudioReaderFactoryManager.TryGetFactory(type, out var factory)
            ? create(factory)
            : default;

    private static bool TryGetStream(this AudioReaderFactoryResult result, [NotNullWhen(true)] out WaveStream? stream)
    {
        stream = result.Stream;
        return stream != null;
    }

    private static bool StreamAndProvider(this AudioReaderFactoryResult result, [NotNullWhen(true)] out WaveStream? stream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        if (result is ({ } resultStream, { } resultProvider))
        {
            (stream, provider) = (resultStream, resultProvider);
            return true;
        }

        stream = null;
        provider = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a <see cref="WaveStream"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <param name="stream">The resulting <see cref="WaveStream"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.</param>
    /// <returns>Whether a <see cref="WaveStream"/> was successfully created.</returns>
    public static bool Stream(string path, [NotNullWhen(true)] out WaveStream? stream)
    {
        if (GetResult(Path.GetExtension(path), factory => factory.FromPath(path)).TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a <see cref="WaveStream"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="source">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the <see cref="WaveStream"/>.</param>
    /// <param name="stream">The resulting <see cref="WaveStream"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.</param>
    /// <returns>Whether a <see cref="WaveStream"/> was successfully created.</returns>
    /// <remarks>The period is automatically trimmed from the start of the <paramref name="fileType"/>.</remarks>
    public static bool Stream(Stream source, string fileType, bool closeOnDispose, [NotNullWhen(true)] out WaveStream? stream)
    {
        if (GetResult(fileType, factory => factory.FromStream(source, closeOnDispose)).TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    /// <summary>
    /// Attempts to create an <see cref="ISampleProvider"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="convertStream">Whether to convert the <see cref="WaveStream"/> to an <see cref="ISampleProvider"/> if the factory only returns a <see cref="WaveStream"/>.</param>
    /// <param name="provider">The resulting <see cref="ISampleProvider"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return an <see cref="ISampleProvider"/> and <paramref name="convertStream"/> is false.</param>
    /// <returns>Whether an <see cref="ISampleProvider"/> was successfully created.</returns>
    /// <remarks>The period is automatically trimmed from the start of the <paramref name="fileType"/>.</remarks>
    public static bool Provider(Stream stream, string fileType, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        provider = GetResult(fileType, factory => factory.FromStream(stream, false)) switch
        {
            (_, { } resultProvider) => resultProvider,
            ({ } resultStream, _) when convertStream => resultStream.ToSampleProvider(),
            _ => null
        };
        return provider != null;
    }

    /// <summary>
    /// Attempts to create a <see cref="WaveStream"/> and its converted <see cref="ISampleProvider"/> from the given file path.
    /// </summary>
    /// <param name="path">The file path to read the audio from.</param>
    /// <param name="stream">The resulting <see cref="WaveStream"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.</param>
    /// <param name="provider">The resulting <see cref="ISampleProvider"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return an <see cref="ISampleProvider"/>.</param>
    /// <returns>Whether a <see cref="WaveStream"/> and its corresponding <see cref="ISampleProvider"/> were successfully created.</returns>
    public static bool StreamAndProvider(string path, [NotNullWhen(true)] out WaveStream? stream, [NotNullWhen(true)] out ISampleProvider? provider)
        => GetResult(Path.GetExtension(path), factory => factory.FromPath(path)).StreamAndProvider(out stream, out provider);

    /// <summary>
    /// Attempts to create a <see cref="WaveStream"/> and its converted <see cref="ISampleProvider"/> from the given <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <param name="source">The <see cref="System.IO.Stream"/> to read the audio from.</param>
    /// <param name="fileType">The file type of the audio in the stream, e.g. "wav", "aiff".</param>
    /// <param name="closeOnDispose">Whether to close the stream when disposing the <see cref="WaveStream"/>.</param>
    /// <param name="stream">The resulting <see cref="WaveStream"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.</param>
    /// <param name="provider">The resulting <see cref="ISampleProvider"/> if successful. <see langword="null"/> if no factory was found for the file type, or if the factory didn't return an <see cref="ISampleProvider"/>.</param>
    /// <returns>Whether a <see cref="WaveStream"/> and its corresponding <see cref="ISampleProvider"/> were successfully created.</returns>
    /// <remarks>The period is automatically trimmed from the start of the <paramref name="fileType"/>.</remarks>
    public static bool StreamAndProvider(
        Stream source,
        string fileType,
        bool closeOnDispose,
        [NotNullWhen(true)] out WaveStream? stream,
        [NotNullWhen(true)] out ISampleProvider? provider
    ) => GetResult(fileType, factory => factory.FromStream(source, closeOnDispose)).StreamAndProvider(out stream, out provider);

}
