using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NVorbis;

public sealed class NVorbisPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.NVorbis";
    public override string Description => "Registers NVorbis support";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public static void RegisterFactory() => AudioReaderFactoryManager.RegisterFactory("ogg", new VorbisStreamFactory());

    public override void Enable() => RegisterFactory();

    public override void Disable()
    {
    }

}
