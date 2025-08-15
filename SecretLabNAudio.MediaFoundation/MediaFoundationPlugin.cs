using System.Collections.Generic;
using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.MediaFoundation;

public sealed class MediaFoundationPlugin : Plugin
{

    public static IReadOnlyCollection<string> SupportedFormats { get; } =
    [
        ".3g2",
        ".3gp",
        ".3gp2",
        ".3gpp",
        ".asf",
        ".wma",
        ".wmv",
        ".aac",
        ".adts",
        ".avi",
        ".mp3",
        ".m4a",
        ".m4v",
        ".mov",
        ".mp4",
        ".sami",
        ".smi"
    ];

    public override string Name => "SecretLabNAudio.MediaFoundation";

    public override string Description => "Registers Media Foundation support";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public static void RegisterFactory()
    {
        Ensure<MediaFoundationReader>();
        var factory = new MediaFoundationFactory();
        foreach (var format in SupportedFormats)
            AudioReaderFactoryManager.RegisterFactory(format, factory);
    }

    private static void Ensure<T>() => _ = typeof(T);

    public override void Enable() => RegisterFactory();

    public override void Disable()
    {
    }

}
