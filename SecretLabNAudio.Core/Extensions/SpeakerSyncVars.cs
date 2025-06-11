using SpeakerToy = AdminToys.SpeakerToy;

namespace SecretLabNAudio.Core.Extensions;

using SpeakerSyncVarData = (float? Volume, bool? IsSpatial, float? MinDistance, float? MaxDistance);

public static class SpeakerSyncVars
{

    private const ulong IsSpatialBit = 64UL;
    private const ulong VolumeBit = 128UL;
    private const ulong MinDistanceBit = 256UL;
    private const ulong MaxDistanceBit = 512UL;

    public static void SendFakeSyncVars(NetworkConnectionToClient connection, SpeakerToy speaker, SpeakerSyncVarData data)
    {
        if (data == default)
            return;
        using var writer = NetworkWriterPool.Get();
        SerializeServer(speaker, writer, data);
        connection.Send(new EntityStateMessage
        {
            netId = speaker.netId,
            payload = writer.ToArraySegment()
        });
    }

    private static void SerializeServer(SpeakerToy speaker, NetworkWriter observersWriter, SpeakerSyncVarData data)
    {
        var networkBehaviours = speaker.netIdentity.NetworkBehaviours;
        for (var i = 0; i < networkBehaviours.Length; i++)
        {
            if (networkBehaviours[i] != speaker)
                continue;
            Compression.CompressVarUInt(observersWriter, (uint) (1 << i));
            Serialize(speaker, observersWriter, data);
            break;
        }
    }

    private static void Serialize(SpeakerToy speaker, NetworkWriter writer, SpeakerSyncVarData data)
    {
        var start = writer.Position;
        writer.WriteByte(0);
        var dataStart = writer.Position;
        try
        {
            writer.WriteULong(0); // SerializeSyncObjects
            var dirtyBits = GetDirtyBits(data);
            // AdminToyBase.SerializeSyncVars
            writer.WriteULong(dirtyBits);
            // SpeakerToy.SerializeSyncVars
            writer.WriteULong(dirtyBits);
            if (data.IsSpatial.HasValue)
                writer.WriteBool(data.IsSpatial.Value);
            if (data.Volume.HasValue)
                writer.WriteFloat(data.Volume.Value);
            if (data.MinDistance.HasValue)
                writer.WriteFloat(data.MinDistance.Value);
            if (data.MaxDistance.HasValue)
                writer.WriteFloat(data.MaxDistance.Value);
        }
        catch (Exception ex)
        {
            Debug.LogError($"OnSerialize failed for: object={speaker.name} component={speaker.GetType()} sceneId={speaker.netIdentity.sceneId:X}\n\n{ex}");
        }

        var dataEnd = writer.Position;
        writer.Position = start;
        var num = (byte) (dataEnd - dataStart & byte.MaxValue);
        writer.WriteByte(num);
        writer.Position = dataEnd;
    }

    private static ulong GetDirtyBits(SpeakerSyncVarData data)
    {
        var dirtyBits = 0UL;
        if (data.Volume.HasValue)
            dirtyBits |= VolumeBit;
        if (data.IsSpatial.HasValue)
            dirtyBits |= IsSpatialBit;
        if (data.MinDistance.HasValue)
            dirtyBits |= MinDistanceBit;
        if (data.MaxDistance.HasValue)
            dirtyBits |= MaxDistanceBit;
        return dirtyBits;
    }

}
