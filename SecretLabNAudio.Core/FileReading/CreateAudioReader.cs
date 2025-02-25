using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

public static class CreateAudioReader
{

    private static AudioReaderFactoryResult Result(string path, string type)
        => AudioReaderFactoryManager.GetFactory(type).FromPath(path);

    private static AudioReaderFactoryResult Result(Stream source, string type, bool closeOnDispose)
        => AudioReaderFactoryManager.GetFactory(type).FromStream(source, closeOnDispose);

    private static WaveStream GetStream(this AudioReaderFactoryResult result, string fileType)
        => result is IWaveStreamResult waveStreamResult
            ? waveStreamResult.Stream
            : throw new NotSupportedException($"Factory for {fileType} did not return a WaveStream");

    private static ISampleProvider GetProvider(this AudioReaderFactoryResult result, string fileType, bool convertStream) => result switch
    {
        ISampleProviderResult sampleProviderResult => sampleProviderResult.Provider,
        IWaveStreamResult waveStreamResult when convertStream => waveStreamResult.Stream.ToSampleProvider(),
        _ => throw new NotSupportedException($"Factory for {fileType} did not return a SampleProvider")
    };

    public static WaveStream Stream(string path)
    {
        var type = Path.GetExtension(path);
        return Result(path, type).GetStream(type);
    }

    public static WaveStream Stream(Stream source, string fileType, bool closeOnDispose = true)
        => Result(source, fileType, closeOnDispose).GetStream(fileType);

    public static ISampleProvider Provider(string path, bool convertStream = true)
    {
        var type = Path.GetExtension(path);
        return Result(path, type).GetProvider(type, convertStream);
    }

    public static ISampleProvider Provider(Stream stream, string fileType, bool closeOnDispose = true, bool convertStream = true)
        => Result(stream, fileType, closeOnDispose).GetProvider(fileType, convertStream);

    public static StreamAndProviderResult StreamAndProvider(string path)
    {
        var type = Path.GetExtension(path);
        return Result(path, type) as StreamAndProviderResult ?? throw new NotSupportedException($"Factory for {type} did not return a StreamAndProviderResult");
    }

    public static StreamAndProviderResult StreamAndProvider(Stream source, string fileType, bool closeOnDispose = true)
        => Result(source, fileType, closeOnDispose) as StreamAndProviderResult
           ?? throw new NotSupportedException($"Factory for {fileType} did not return a StreamAndProviderResult");

}
