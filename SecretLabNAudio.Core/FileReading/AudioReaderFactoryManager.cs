using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SecretLabNAudio.Core.FileReading;

public static class AudioReaderFactoryManager
{

    private static readonly Dictionary<string, IAudioReaderFactory> Factories = [];

    static AudioReaderFactoryManager() => RegisterFactory("wav", new WaveReaderFactory());

    private static string Sanitize(this string fileType) => fileType.TrimStart('.').ToLower();

    public static void RegisterFactory(string fileType, IAudioReaderFactory factory)
        => Factories[fileType.Sanitize()] = factory;

    public static bool TryGetFactory(string fileType, [NotNullWhen(true)] out IAudioReaderFactory? factory)
        => Factories.TryGetValue(fileType.Sanitize(), out factory);

    public static IAudioReaderFactory GetFactory(string fileType)
        => TryGetFactory(fileType, out var factory)
            ? factory
            : throw new NotSupportedException($"No factory registered for file type {fileType}");

}
