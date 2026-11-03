using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerBaseExtensions
{

    extension<T>(T player) where T : AudioPlayerBase
    {

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SendFilter"/> of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="filter">The filter to broadcast audio with.</param>
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

        public T WithPacketOutput(AudioPacketOutput output)
        {
            player.Output = output;
            return player;
        }

        public T PoolOnEnd()
        {
            player.EndAction = EndAction.Pool;
            return player;
        }

        public T DestroyOnEnd()
        {
            player.EndAction = EndAction.Destroy;
            return player;
        }

        public T StopOnEnd()
        {
            player.EndAction = EndAction.Stop;
            return player;
        }

        /// <summary>
        /// Pauses (or unpauses) the audio player.
        /// </summary>
        /// <param name="pause">Whether to pause the player.</param>
        /// <returns>The player itself.</returns>
        public T Pause(bool pause = true)
        {
            player.IsPaused = pause;
            return player;
        }

        /// <summary>
        /// Unpauses the audio player.
        /// </summary>
        /// <returns>The player itself.</returns>
        public T Resume() => player.Pause(false);

        public T Stop()
        {
            player.Stop();
            return player;
        }

    }

    extension(AudioPlayerBase player)
    {

        public bool TryGetSpeaker([NotNullWhen(true)] out SpeakerToy? speaker)
        {
            speaker = player.OutputSpeaker;
            return speaker != null;
        }

        internal SpeakerToy? OutputSpeaker => (player.Output as SpeakerToyOutput)?.Speaker;

    }

}
