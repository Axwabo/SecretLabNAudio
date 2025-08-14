using System.Collections.Generic;
using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.MediaFoundation;

/// <summary>A plugin that registers Media Foundation support for SecretLabNAudio.Core. This plugin is Windows-specific.</summary>
public sealed class MediaFoundationPlugin : Plugin
{

    /// <summary>The file formats supported by the <see cref="MediaFoundationReader"/>.</summary>
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

    /// <inheritdoc />
    public override string Name => "SecretLabNAudio.MediaFoundation";

    /// <inheritdoc />
    public override string Description => "Registers Media Foundation support";

    /// <inheritdoc />
    public override string Author => "Axwabo";

    /// <inheritdoc />
    public override Version Version => GetType().Assembly.GetName().Version;

    /// <inheritdoc />
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    /// <summary>Registers a factory to read audio from files and other streams using Media Foundation.</summary>
    /// <seealso cref="SupportedFormats"/>
    public static void RegisterFactory()
    {
        Ensure<MediaFoundationReader>();
        var factory = new MediaFoundationFactory();
        foreach (var format in SupportedFormats)
            AudioReaderFactoryManager.RegisterFactory(format, factory);
    }

    private static void Ensure<T>() => _ = typeof(T);

    /// <summary>Calls <see cref="RegisterFactory"/>.</summary>
    public override void Enable() => RegisterFactory();

    /// <inheritdoc />
    public override void Disable()
    {
    }

}
