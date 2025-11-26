using System.Diagnostics.CodeAnalysis;
using System.IO;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.FileReading;

public static class TryCreateAudioProcessor
{

    private static bool TryCreate(string type, Func<IAudioReaderFactory, AudioReaderFactoryResult> create, [NotNullWhen(true)] out StreamAudioProcessor? processor)
    {
        if (!AudioReaderFactoryManager.TryGetFactory(type, out var factory))
        {
            processor = null;
            return false;
        }

        processor = create(factory) switch
        {
            ({ } stream, { } provider) => new StreamAudioProcessor(stream, provider),
            ({ } stream, _) => new StreamAudioProcessor(stream),
            _ => null
        };
        return processor != null;
    }

    public static bool FromFile(string path, [NotNullWhen(true)] out StreamAudioProcessor? processor)
        => TryCreate(Path.GetExtension(path), factory => factory.FromPath(path), out processor);

    public static bool FromStream(Stream baseStream, string fileType, bool isOwned, [NotNullWhen(true)] out StreamAudioProcessor? processor)
        => TryCreate(Path.GetExtension(fileType), factory => factory.FromStream(baseStream, isOwned), out processor);

}
