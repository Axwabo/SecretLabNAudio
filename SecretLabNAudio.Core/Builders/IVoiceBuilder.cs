using SecretLabNAudio.Core.Outputs;
using VoiceChat;

namespace SecretLabNAudio.Core.Builders;

public interface IVoiceBuilder
{

    PlayerVoiceOutput Output { get; }

}

public static class VoiceBuilderExtensions
{

    extension<T>(T builder) where T : IVoiceBuilder
    {

        public T WithChannel(VoiceChatChannel channel)
        {
            builder.Output.Channel = channel;
            return builder;
        }

    }

}
