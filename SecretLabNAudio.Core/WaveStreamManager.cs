using System.Collections.Generic;
using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core;

public delegate WaveStream WaveStreamFromStream(Stream stream, bool closeOnDispose = false);

public delegate WaveStream WaveStreamFromPath(string path);

public static class WaveStreamManager
{

    private static readonly Dictionary<string, (WaveStreamFromStream?, WaveStreamFromPath?)> Factories = [];

    static WaveStreamManager() => RegisterFactory("wav", (stream, _) => new WaveFileReader(stream), path => new WaveFileReader(path));

    private static string Sanitize(this string fileType) => fileType.TrimStart('.').ToLower();

    public static void RegisterFactory(string fileType, WaveStreamFromStream fromStream)
        => Factories[fileType.Sanitize()] = (fromStream, null);

    public static void RegisterFactory(string fileType, WaveStreamFromPath fromPath)
        => Factories[fileType.Sanitize()] = (null, fromPath);

    public static void RegisterFactory(string fileType, WaveStreamFromStream fromStream, WaveStreamFromPath fromPath)
        => Factories[fileType.Sanitize()] = (fromStream, fromPath);

    public static bool TryGetFactories(string fileType, out WaveStreamFromStream? fromStream, out WaveStreamFromPath? fromPath)
    {
        if (Factories.TryGetValue(fileType.Sanitize(), out var pair))
        {
            (fromStream, fromPath) = pair;
            return true;
        }

        fromStream = null;
        fromPath = null;
        return false;
    }

    public static WaveStream Create(string path)
    {
        var (fromStream, fromPath) = Factories[Path.GetExtension(path).Sanitize()];
        if (fromPath != null)
            return fromPath(path);
        var stream = File.OpenRead(path);
        return fromStream!(stream, true);
    }

    public static WaveStream Create(Stream stream, string fileType)
    {
        var sanitized = fileType.Sanitize();
        var (fromStream, _) = Factories[sanitized];
        return fromStream != null
            ? fromStream(stream)
            : throw new InvalidOperationException($"Stream-based creation is not supported for file type {sanitized}");
    }

}
