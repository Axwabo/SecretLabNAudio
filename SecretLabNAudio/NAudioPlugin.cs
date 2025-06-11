using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.NLayer;
using SecretLabNAudio.NVorbis;

namespace SecretLabNAudio;

/// <summary>A plugin that includes SecretLabNAudio.Core, NVorbis and NAudio support with dependencies.</summary>
public sealed class NAudioPlugin : Plugin
{

    /// <inheritdoc/>
    public override string Name => "SecretLabNAudio";

    /// <inheritdoc/>
    public override string Description => "SecretLabNAudio.Core, SecretLabNAudio.NVorbis and SecretLabNAudio.NLayer";

    /// <inheritdoc/>
    public override string Author => "Axwabo";

    /// <inheritdoc/>
    public override Version Version => GetType().Assembly.GetName().Version;

    /// <inheritdoc/>
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    /// <inheritdoc/>
    public override void Enable()
    {
        NVorbisPlugin.RegisterFactory();
        NLayerPlugin.RegisterFactory();
    }

    /// <inheritdoc/>
    public override void Disable()
    {
    }

}
