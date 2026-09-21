using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerBaseExtensions
{

    extension<T>(T player) where T : AudioPlayerBase
    {

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SendFilter"/> of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="filter">The filter to send audio with.</param>
        /// <returns>The player itself.</returns>
        public T WithSendFilter(SendFilter filter)
        {
            player.SendFilter = filter;
            return player;
        }

        public T WithSendFilter(Func<Player, bool> filter)
        {
            player.SendFilter = new DelegateFilter(filter);
            return player;
        }

        public T PatchSpeaker(Action<SpeakerToy> action)
        {
            if (player.Output is SpeakerToyOutput {Speaker: var speaker})
                action(speaker);
            return player;
        }

        public T WithPacketOutput(AudioPacketOutput output)
        {
            player.Output = output;
            return player;
        }

    }

    extension(AudioPlayerBase player)
    {

        public SpeakerToy? OutputSpeaker => (player.Output as SpeakerToyOutput)?.Speaker;

    }

}
