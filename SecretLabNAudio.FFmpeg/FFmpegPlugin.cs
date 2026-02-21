using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.FFmpeg.Installer;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg;

internal sealed class FFmpegPlugin : Plugin<FFmpegConfig>
{

    public static FFmpegPlugin? Instance { get; private set; }

    public override string Name => "SecretLabNAudio.FFmpeg";
    public override string Description => "FFmpeg support for SecretLabNAudio";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable()
    {
        Instance = this;
        if (Config != null)
            FFmpegSL.Path = Config.Path;
        if (!FFmpegInstaller.TryFindExisting(out var destination) || FFmpegSL.Path == destination || FFmpegInstaller.IsInstalled)
            return;
        Logger.Info("Found FFmpeg on disk, overriding configuration");
        FFmpegInstaller.OverrideConfig(destination);
    }

    public override void Disable()
    {
    }

}
