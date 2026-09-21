using NAudio.Wave.SampleProviders;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="AudioPlayer"/> class.</summary>
public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to apply the settings to.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Sets the master amplification of the <see cref="AudioPlayer"/>.
        /// This is different from <see cref="WithVolume"/>, which changes the volume of the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="scalar">The scalar to multiply samples by before encoding. 1 is normal volume, 2 is double volume, 0.5 is half volume, etc.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// This can be used to amplify audio without requiring a <see cref="VolumeSampleProvider"/>.
        /// The <see cref="AudioPlayer.OutputMonitor"/> is not affected by this.
        /// </remarks>
        public AudioPlayer WithMasterAmplification(float scalar)
        {
            player.MasterAmplification = scalar;
            return player;
        }

        /// <summary>
        /// Pauses (or unpauses) the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="pause">Whether to pause the player.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer Pause(bool pause = true)
        {
            player.IsPaused = pause;
            return player;
        }

        /// <summary>
        /// Unpauses the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <returns>The player itself.</returns>
        public AudioPlayer Resume() => player.Pause(false);

        /// <summary>
        /// Sets <seealso cref="AudioPlayer.SampleProvider"/> to <see langword="null"/>, effectively stopping playback.
        /// </summary>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithoutProvider()
        {
            player.SampleProvider = null;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.OutputMonitor"/> of the <see cref="AudioPlayer"/> to the given <see cref="IAudioPacketMonitor"/>.
        /// </summary>
        /// <param name="monitor">The monitor to set.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithOutputMonitor(IAudioPacketMonitor monitor)
        {
            player.OutputMonitor = monitor;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.OwnsProvider"/> of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the <see cref="AudioPlayer.SampleProvider"/> when the provider is changed or the player is pooled/destroyed.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithProviderOwnership(bool isOwned = true)
        {
            player.OwnsProvider = isOwned;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> to <see langword="null"/> of the <see cref="AudioPlayer"/> when no samples are read.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <seealso cref="AudioPlayer.AlwaysRead"/>
        public AudioPlayer UnsetProviderOnEnd()
        {
            player.AlwaysRead = false;
            return player;
        }

        /// <summary>
        /// Destroys the <see cref="AudioPlayer"/> when the provider ends.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <seealso cref="AudioPlayer.Ended"/>
        public AudioPlayer DestroyOnEnd()
        {
            player.Ended += () =>
            {
                if (player.OutputSpeaker is { } speaker)
                    speaker.Destroy();
                Object.Destroy(player);
            };
            return player;
        }

        /// <summary>
        /// Returns the <see cref="AudioPlayer"/> to the pool when the provider ends.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <seealso cref="AudioPlayer.Ended"/>
        public AudioPlayer PoolOnEnd()
        {
            player.Ended += () => { AudioPlayerPool.Return(player); };
            return player;
        }

    }

}
