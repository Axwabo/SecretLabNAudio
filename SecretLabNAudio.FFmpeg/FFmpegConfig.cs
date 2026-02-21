using System.ComponentModel;

namespace SecretLabNAudio.FFmpeg;

[Serializable]
internal sealed class FFmpegConfig
{

    [Description("Location of the ffmpeg executable. May be the name itself if it's in PATH")]
    public string Path { get; set; } = "ffmpeg";

    [Description("Whether to prevent overriding the configured FFmpeg path when the plugin is enabled")]
    public bool DoNotOverrideOnEnable { get; set; }

}
