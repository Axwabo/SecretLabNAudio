using System.Runtime.CompilerServices;

namespace SecretLabNAudio.Core.Encoded;

[InlineArray(AudioConstants.MaxEncodedBytesPerPacket)]
public struct VoiceChatOpusPacket
{

    private byte _value;

}
