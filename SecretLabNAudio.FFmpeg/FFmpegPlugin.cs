using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.FFmpeg.Installer;

namespace SecretLabNAudio.FFmpeg;

internal sealed class FFmpegPlugin : Plugin<FFmpegConfig>
{

    private const string Install = "Use the \"installFFmpeg\" command to install FFmpeg.";
    private const string UnknownError = $"The configured FFmpeg installation doesn't exist or isn't an FFmpeg executable! {Install}";
    private const string NotFound = $"The configured FFmpeg installation cannot be found! {Install}";
    private const string AccessDenied = "The configured FFmpeg installation cannot be launched because access is denied. Use the \"chmodFFmpeg\" command to make it executable.";

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
        if ((!Config?.DoNotOverrideOnEnable ?? true) && FFmpegInstaller.TryFindExisting(out var destination) && FFmpegSL.Path != destination && !FFmpegInstaller.IsInstalled())
        {
            Logger.Info("Found FFmpeg on disk, overwriting configuration");
            FFmpegInstaller.OverwriteConfig(destination);
        }

        if (!FFmpegInstaller.IsInstalled())
            Logger.Error(FFmpegSL.LastCaughtStartError switch
            {
                NativeErrorCode.FileNotFound or NativeErrorCode.PathNotFound => NotFound,
                NativeErrorCode.AccessDenied => AccessDenied,
                _ => UnknownError
            });
    }

    public override void Disable()
    {
    }

}
