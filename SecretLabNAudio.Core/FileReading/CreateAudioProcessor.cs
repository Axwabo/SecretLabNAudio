namespace SecretLabNAudio.Core.FileReading;

public static class CreateAudioProcessor
{

    private static StreamAudioProcessor Convert(this AudioReaderFactoryResult result, string type) => result switch
    {
        ({ } stream, { } provider) => new StreamAudioProcessor(stream, provider),
        ({ } stream, null) => new StreamAudioProcessor(stream),
        _ => throw new NotSupportedException($"Factory for {type} did not return a WaveStream")
    };

    public static StreamAudioProcessor FromFile(string path)
    {
        var type = Path.GetExtension(path);
        return AudioReaderFactoryManager.GetFactory(type).FromPath(path).Convert(type);
    }

    public static StreamAudioProcessor FromStream(Stream baseStream, string fileType, bool isOwned = true)
        => AudioReaderFactoryManager.GetFactory(fileType).FromStream(baseStream, isOwned).Convert(fileType);

}
