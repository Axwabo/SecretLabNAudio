namespace SecretLabNAudio.Core;

public static class Extensions
{

    public static void ApplySettings(this AudioPlayer player, AudioPlayerSettings settings)
    {
        player.Speaker.NetworkIsSpatial = settings.IsSpatial;
        player.Speaker.NetworkVolume = settings.Volume;
        player.Speaker.NetworkMinDistance = settings.MinDistance;
        player.Speaker.NetworkMaxDistance = settings.MaxDistance;
    }

}
