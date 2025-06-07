using System.Diagnostics.CodeAnalysis;
using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

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

    private static bool TryGetProvider(this AudioReaderFactoryResult result, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        provider = result switch
        {
            (_, not null) => result.Provider,
            ({ } stream, _) when convertStream => stream.ToSampleProvider(),
            _ => null
        };
        return provider != null;
    }

    public static bool Stream(string path, [NotNullWhen(true)] out WaveStream? stream)
    {
        var type = Path.GetExtension(path);
        if (GetResult(type, factory => factory.FromPath(path)).TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    public static bool Stream(Stream source, string fileType, bool closeOnDispose, [NotNullWhen(true)] out WaveStream? stream)
    {
        if (GetResult(fileType, factory => factory.FromStream(source, closeOnDispose)).TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    public static bool Provider(string path, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var type = Path.GetExtension(path);
        if (GetResult(type, factory => factory.FromPath(path)).TryGetProvider(convertStream, out provider))
            return true;
        provider = null;
        return false;
    }

    public static bool Provider(Stream stream, string fileType, bool closeOnDispose, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        if (GetResult(fileType, factory => factory.FromStream(stream, closeOnDispose)).TryGetProvider(convertStream, out provider))
            return true;
        provider = null;
        return false;
    }

    public static bool StreamAndProvider(string path, [NotNullWhen(true)] out WaveStream? stream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var type = Path.GetExtension(path);
        if (GetResult(type, factory => factory.FromPath(path)) is ({ } resultStream, { } resultProvider))
        {
            (stream, provider) = (resultStream, resultProvider);
            return true;
        }

        stream = null;
        provider = null;
        return false;
    }

}
