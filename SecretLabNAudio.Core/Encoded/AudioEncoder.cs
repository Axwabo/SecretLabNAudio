using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using static SecretLabNAudio.Core.AudioConstants;

namespace SecretLabNAudio.Core.Encoded;

public static class AudioEncoder
{

    [ThreadStatic]
    private static float[]? _audioBuffer;

    [ThreadStatic]
    private static byte[]? _encodedBuffer;

    public static void Encode(ISampleProvider provider, Stream destination, OpusApplicationType preset = OpusApplicationType.Audio)
    {
        using var encoder = new OpusEncoder(preset);
        var audio = _audioBuffer ??= new float[SamplesPerPacket];
        var encoded = _encodedBuffer ??= new byte[MaxEncodedBytesPerPacket];
        int read;
        while ((read = provider.Read(audio, 0, SamplesPerPacket)) > 0)
        {
            var length = encoder.Encode(audio, encoded, read);
            destination.WriteByte((byte) length); //todo
            destination.Write(encoded, 0, length);
        }
    }

}
