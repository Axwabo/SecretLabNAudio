using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using NAudio.Vorbis;
using NVorbis;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NVorbis;

public sealed class NVorbisPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.NVorbis";

    public override string Description => "Registers NVorbis support";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override LoadPriority Priority => LoadPriority.High;

    public static void RegisterFactory()
    {
        Ensure<VorbisReader>();
        Ensure<VorbisWaveReader>();
        AudioReaderFactoryManager.RegisterFactory("ogg", new VorbisStreamFactory());
    }

    private static void Ensure<T>() => _ = typeof(T);

    public override void Enable() => RegisterFactory();

    public override void Disable()
    {
    }

}
