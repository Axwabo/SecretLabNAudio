using LabApi.Loader.Features.Plugins;
using NAudio.Vorbis;
using NVorbis;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NVorbis;

/// <summary>A plugin that registers NVorbis support for SecretLabNAudio.Core.</summary>
public sealed class NVorbisPlugin : Plugin
{

    /// <inheritdoc/>
    public override string Name => "SecretLabNAudio.NVorbis";

    /// <inheritdoc/>
    public override string Description => "Registers NVorbis support";

    /// <inheritdoc/>
    public override string Author => "Axwabo";

    /// <inheritdoc/>
    public override Version Version => GetType().Assembly.GetName().Version;

    /// <inheritdoc/>
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    /// <summary>Registers a factory to read Ogg Vorbis files.</summary>
    public static void RegisterFactory()
    {
        Ensure<VorbisReader>();
        Ensure<VorbisWaveReader>();
        AudioReaderFactoryManager.RegisterFactory("ogg", new VorbisStreamFactory());
    }

    private static void Ensure<T>() => _ = typeof(T);

    /// <summary>Calls <see cref="RegisterFactory"/>.</summary>
    public override void Enable() => RegisterFactory();

    /// <inheritdoc/>
    public override void Disable()
    {
    }

}
