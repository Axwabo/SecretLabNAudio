using LabApi.Loader.Features.Plugins;
using NLayer;
using NLayer.NAudioSupport;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NLayer;

/// <summary>A plugin that registers NLayer support for SecretLabNAudio.Core.</summary>
public sealed class NLayerPlugin : Plugin
{

    /// <inheritdoc/>
    public override string Name => "SecretLabNAudio.NLayer";

    /// <inheritdoc/>
    public override string Description => "Registers NLayer support";

    /// <inheritdoc/>
    public override string Author => "Axwabo";

    /// <inheritdoc/>
    public override Version Version => GetType().Assembly.GetName().Version;

    /// <inheritdoc/>
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    /// <summary>Registers a factory to read MP3 files.</summary>
    public static void RegisterFactory()
    {
        Ensure<MpegFile>();
        Ensure<Mp3FrameDecompressor>();
        AudioReaderFactoryManager.RegisterFactory("mp3", new MpegStreamFactory());
    }

    private static void Ensure<T>() => _ = typeof(T);

    /// <summary>Calls <see cref="RegisterFactory"/>.</summary>
    public override void Enable() => RegisterFactory();

    /// <inheritdoc/>
    public override void Disable()
    {
    }

}
