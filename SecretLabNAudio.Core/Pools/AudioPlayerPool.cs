using SecretLabNAudio.Core.Extensions;
using VoiceChat.Playbacks;

namespace SecretLabNAudio.Core.Pools;

public class AudioPlayerPool
{

    public static byte NextAvailableId
    {
        get
        {
            byte available = 0;
            foreach (var instance in SpeakerToyPlaybackBase.AllInstances)
                if (available == instance.ControllerId)
                    available++;
            return available;
        }
    }

    public static AudioPlayer Rent(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
    {
        if (!SpeakerToyPool.TryGetFromPool(out var existing, parent, position, false))
            return AudioPlayer.Create(id, settings, parent, position);
        var player = existing.WithId(id)
            .ApplySettings(settings)
            .AddAudioPlayer();
        NetworkServer.Spawn(player.gameObject);
        return player;
    }

    public static AudioPlayer Rent(SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
        => Rent(NextAvailableId, settings, parent, position);

}
