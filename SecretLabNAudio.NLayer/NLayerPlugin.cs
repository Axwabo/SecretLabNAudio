using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NLayer;

public sealed class NLayerPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.NLayer";
    public override string Description => "Registers NLayer support";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public static void RegisterFactory() => AudioReaderFactoryManager.RegisterFactory("mp3", new MpegStreamFactory());

    public override void Enable() => RegisterFactory();

    public override void Disable()
    {
    }

}
