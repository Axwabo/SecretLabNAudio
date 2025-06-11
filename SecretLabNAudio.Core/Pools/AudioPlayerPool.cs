using VoiceChat.Playbacks;

namespace SecretLabNAudio.Core.Pools;

public static class AudioPlayerPool
{

    private static bool[] Occupied = new bool[byte.MaxValue + 1];

    public static byte NextAvailableId
    {
        get
        {
            Array.Clear(Occupied, 0, Occupied.Length);
            foreach (var instance in SpeakerToyPlaybackBase.AllInstances)
                Occupied[instance.ControllerId] = true;
            for (var i = 0; i < Occupied.Length; i++)
                if (!Occupied[i])
                    return (byte) i;
            throw new OverflowException("No available IDs found");
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

    public static void Return(AudioPlayer player) => SpeakerToyPool.Return(player.Speaker);

}
