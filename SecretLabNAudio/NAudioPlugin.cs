using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.NLayer;
using SecretLabNAudio.NVorbis;

namespace SecretLabNAudio;

public sealed class NAudioPlugin : Plugin
{

    public override string Name => "SecretLabNAudio";

    public override string Description => "SecretLabNAudio.Core, SecretLabNAudio.NVorbis and SecretLabNAudio.NLayer";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable()
    {
        NVorbisPlugin.RegisterFactory();
        NLayerPlugin.RegisterFactory();
    }

    public override void Disable()
    {
    }

}
