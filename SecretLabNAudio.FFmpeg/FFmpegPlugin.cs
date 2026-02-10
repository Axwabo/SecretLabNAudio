using LabApi.Loader.Features.Plugins;

namespace SecretLabNAudio.FFmpeg;

public sealed class FFmpegPlugin : Plugin<FFmpegConfig>
{

    internal static FFmpegPlugin? Instance { get; private set; }

    public override string Name => "SecretLabNAudio.FFmpeg";
    public override string Description => "FFmpeg support for SecretLabNAudio";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable()
    {
        if (Config != null)
            FFmpegSL.Path = Config.Path;
        Instance = this;
    }

    public override void Disable()
    {
    }

}
