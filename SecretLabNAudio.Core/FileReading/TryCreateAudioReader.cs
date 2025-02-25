using System.Diagnostics.CodeAnalysis;
using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

public static class TryCreateAudioReader
{

    private static bool TryGetResult(string type, Func<IAudioReaderFactory, AudioReaderFactoryResult> create, [NotNullWhen(true)] out AudioReaderFactoryResult? stream)
    {
        if (AudioReaderFactoryManager.TryGetFactory(type, out var factory))
        {
            stream = create(factory);
            return true;
        }

        stream = null;
        return false;
    }

    private static bool TryGetStream(this AudioReaderFactoryResult result, [NotNullWhen(true)] out WaveStream? stream)
    {
        stream = (result as IWaveStreamResult)?.Stream;
        return stream != null;
    }

    private static bool TryGetProvider(this AudioReaderFactoryResult result, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        provider = result switch
        {
            ISampleProviderResult sampleProviderResult => sampleProviderResult.Provider,
            IWaveStreamResult waveStreamResult when convertStream => waveStreamResult.Stream.ToSampleProvider(),
            _ => null
        };
        return provider != null;
    }

    public static bool Stream(string path, [NotNullWhen(true)] out WaveStream? stream)
    {
        var type = Path.GetExtension(path);
        if (TryGetResult(type, factory => factory.FromPath(path), out var result) && result.TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    public static bool Stream(Stream source, string fileType, bool closeOnDispose, [NotNullWhen(true)] out WaveStream? stream)
    {
        if (TryGetResult(fileType, factory => factory.FromStream(source, closeOnDispose), out var result) && result.TryGetStream(out stream))
            return true;
        stream = null;
        return false;
    }

    public static bool Provider(string path, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var type = Path.GetExtension(path);
        if (TryGetResult(type, factory => factory.FromPath(path), out var result) && result.TryGetProvider(convertStream, out provider))
            return true;
        provider = null;
        return false;
    }

    public static bool Provider(Stream stream, string fileType, bool closeOnDispose, bool convertStream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        if (TryGetResult(fileType, factory => factory.FromStream(stream, closeOnDispose), out var result) && result.TryGetProvider(convertStream, out provider))
            return true;
        provider = null;
        return false;
    }

    public static bool StreamAndProvider(string path, [NotNullWhen(true)] out WaveStream? stream, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var type = Path.GetExtension(path);
        if (TryGetResult(type, factory => factory.FromPath(path), out var result) && result is StreamAndProviderResult streamAndProviderResult)
        {
            (stream, provider) = streamAndProviderResult;
            return true;
        }

        stream = null;
        provider = null;
        return false;
    }

}
