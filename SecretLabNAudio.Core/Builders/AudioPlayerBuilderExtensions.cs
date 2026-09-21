using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Builders;

public static class AudioPlayerBuilderExtensions
{

    extension<T>(T builder) where T : IAudioPlayerBuilder
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

        public T WithOutputMonitor(IAudioPacketMonitor monitor)
        {
            builder.Player.OutputMonitor = monitor;
            return builder;
        }

    }

}
