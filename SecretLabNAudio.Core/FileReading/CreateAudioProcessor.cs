using System.IO;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.FileReading;

public static class CreateAudioProcessor
{

    public static StreamAudioProcessor FromFile(string path, bool convertStream = true)
    {
        var type = Path.GetExtension(path);
        return AudioReaderFactoryManager.GetFactory(type).FromPath(path) switch
        {
            ({ } stream, { } provider) => new StreamAudioProcessor(stream, provider),
            ({ } stream, null) when convertStream => new StreamAudioProcessor(stream),
            _ => throw new NotSupportedException($"Factory for {type} did not return both a stream and a provider")
        };
    }

}
