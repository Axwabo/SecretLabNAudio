using LabApi.Loader.Features.Plugins;

namespace SecretLabNAudio.FFmpeg;

public class FFmpegPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.FFmpeg";
    public override string Description => "FFmpeg support for SecretLabNAudio";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable()
    {
    }

    public override void Disable()
    {
    }

}
