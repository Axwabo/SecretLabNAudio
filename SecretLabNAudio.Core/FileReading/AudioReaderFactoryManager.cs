namespace SecretLabNAudio.Core.FileReading;

/// <summary>Manages <see cref="IAudioReaderFactory">audio reader factories</see>.</summary>
public static class AudioReaderFactoryManager
{

    private static readonly Dictionary<string, IAudioReaderFactory> Factories = new(StringComparer.OrdinalIgnoreCase);

    static AudioReaderFactoryManager()
    {
        var wave = new WaveReaderFactory();
        RegisterFactory("wav", wave);
        RegisterFactory("wave", wave);
        var aiff = new AiffReaderFactory();
        RegisterFactory("aiff", aiff);
        RegisterFactory("aif", aiff);
    }

    /// <summary>
    /// Registers an <see cref="IAudioReaderFactory"/> for the given file type.
    /// </summary>
    /// <param name="fileType">The file type to register the factory for, e.g. ".wav", "aiff".</param>
    /// <param name="factory">The factory to register.</param>
    /// <remarks>The period is automatically trimmed from the start. File type is case-insensitive.</remarks>
    public static void RegisterFactory(string fileType, IAudioReaderFactory factory)
        => Factories[fileType.TrimStart('.')] = factory;

    /// <summary>
    /// Attempts to get the <see cref="IAudioReaderFactory"/> for the given file type.
    /// </summary>
    /// <param name="fileType">The file type to get the factory for, e.g. ".wav", "aiff".</param>
    /// <param name="factory">The factory found for the file type, or <see langword="null"/> if no factory was found.</param>
    /// <returns>Whether a factory was found for the given file type.</returns>
    /// <remarks>The period is automatically trimmed from the start. File type is case-insensitive.</remarks>
    public static bool TryGetFactory(string fileType, [NotNullWhen(true)] out IAudioReaderFactory? factory)
        => Factories.TryGetValue(fileType.TrimStart('.'), out factory);

    /// <summary>
    /// Gets the <see cref="IAudioReaderFactory"/> for the given file type.
    /// </summary>
    /// <param name="fileType">The file type to get the factory for, e.g. ".wav", "aiff".</param>
    /// <returns>The factory for the given file type.</returns>
    /// <exception cref="NotSupportedException">Thrown when no factory is registered for the given file type.</exception>
    /// <remarks>The period is automatically trimmed from the start. File type is case-insensitive.</remarks>
    public static IAudioReaderFactory GetFactory(string fileType)
        => TryGetFactory(fileType, out var factory)
            ? factory
            : throw new NotSupportedException($"No factory registered for file type {fileType}");

    /// <summary>
    /// Checks if a factory exists for the given file type.
    /// </summary>
    /// <param name="fileType">The file type to check.</param>
    /// <returns>Whether a factory has been registered.</returns>
    public static bool HasFactory(string fileType) => Factories.ContainsKey(fileType.TrimStart('.'));

    /// <summary>
    /// Checks if the file exists and if a factory exists for its extension.
    /// </summary>
    /// <param name="filePath">The path to the file to check.</param>
    /// <returns>Whether audio can be read from the file.</returns>
    public static bool IsReadable(string filePath) => File.Exists(filePath) && HasFactory(Path.GetExtension(filePath));

}
