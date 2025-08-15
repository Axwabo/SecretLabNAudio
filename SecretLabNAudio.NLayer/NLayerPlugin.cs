using LabApi.Loader.Features.Plugins;
using NLayer;
using NLayer.NAudioSupport;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NLayer;

public sealed class NLayerPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.NLayer";

    public override string Description => "Registers NLayer support";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public static void RegisterFactory()
    {
        Ensure<MpegFile>();
        Ensure<Mp3FrameDecompressor>();
        AudioReaderFactoryManager.RegisterFactory("mp3", new MpegStreamFactory());
    }

    private static void Ensure<T>() => _ = typeof(T);

    public override void Enable() => RegisterFactory();

    public override void Disable()
    {
    }

}
