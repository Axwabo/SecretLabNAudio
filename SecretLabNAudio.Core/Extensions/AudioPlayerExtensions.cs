namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerExtensions
{

    public static AudioPlayer ApplySettings(this AudioPlayer player, AudioPlayerSettings settings)
    {
        player.IsSpatial = settings.IsSpatial;
        player.Volume = settings.Volume;
        player.MinDistance = settings.MinDistance;
        player.MaxDistance = settings.MaxDistance;
        return player;
    }

}
