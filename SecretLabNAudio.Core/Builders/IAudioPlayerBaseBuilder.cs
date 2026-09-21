using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Builders;

public interface IAudioPlayerBaseBuilder
{

    AudioPlayerBase Player { get; }

}

public static class AudioPlayerBaseBuilderExtensions
{

    extension<T>(T builder) where T : IAudioPlayerBaseBuilder
    {

        public T WithSendFilter(SendFilter filter)
        {
            builder.Player.SendFilter = filter;
            return builder;
        }

        public T WithSendFilter(Func<Player, bool> filter)
        {
            builder.Player.WithSendFilter(filter);
            return builder;
        }

        public T WithPacketOutput(AudioPacketOutput output)
        {
            builder.Player.Output = output;
            return builder;
        }

    }

}
