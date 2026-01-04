using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="AudioPlayer"/> class.</summary>
public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to apply the settings to.</param>
    extension(AudioPlayer player)
    {

        private AudioPlayer PatchSpeaker<T>(Func<SpeakerToy, T, SpeakerToy> method, T parameter)
        {
            method(player.Speaker, parameter);
            return player;
        }

        /// <summary>
        /// Applies the given <see cref="SpeakerSettings"/> to the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="settings">The settings to apply.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer ApplySettings(SpeakerSettings settings)
            => player.PatchSpeaker(SpeakerToyExtensions.ApplySettings, settings);

        /// <summary>
        /// Sets the controller ID of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="id">The ID to set.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithId(byte id)
            => player.PatchSpeaker(SpeakerToyExtensions.WithId, id);

        /// <summary>
        /// Sets the volume of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithVolume(float volume)
            => player.PatchSpeaker(SpeakerToyExtensions.WithVolume, volume);

        /// <summary>
        /// Sets the minimum full volume distance of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="minDistance">The minimum distance to set.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="SpeakerSettings.MinDistance"/>
        public AudioPlayer WithMinDistance(float minDistance)
            => player.PatchSpeaker(SpeakerToyExtensions.WithMinDistance, minDistance);

        /// <summary>
        /// Sets the maximum audible distance of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="maxDistance">The maximum distance to set.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="SpeakerSettings.MaxDistance"/>
        public AudioPlayer WithMaxDistance(float maxDistance)
            => player.PatchSpeaker(SpeakerToyExtensions.WithMaxDistance, maxDistance);

        /// <summary>
        /// Sets whether the <see cref="AudioPlayer"/> is spatial (3D sound).
        /// /// </summary>
        /// <param name="isSpatial">Whether the player should be spatial.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithSpatial(bool isSpatial = true)
            => player.PatchSpeaker(SpeakerToyExtensions.WithSpatial, isSpatial);

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
        /// Sets the <see cref="AudioPlayer.SendEngine"/> of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="engine">The engine to send audio with.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithSendEngine(SendEngine engine)
        {
            player.SendEngine = engine;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SendEngine"/> of the <see cref="AudioPlayer"/> to a <see cref="FilteredSendEngine"/>.
        /// </summary>
        /// <param name="filter">The condition to satisfy for a <see cref="Player"/> to receive the audio.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer WithFilteredSendEngine(Predicate<Player> filter)
            => player.WithSendEngine(new FilteredSendEngine(filter));

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
        /// Destroys the <see cref="AudioPlayer"/> when no samples are read.
        /// </summary>
        /// <returns>The player itself.</returns>
        public AudioPlayer DestroyOnEnd()
        {
            player.NoSamplesRead += player.Destroy;
            return player;
        }

        /// <summary>
        /// Returns the <see cref="AudioPlayer"/> to the pool when no samples are read.
        /// </summary>
        /// <returns>The player itself.</returns>
        public AudioPlayer PoolOnEnd()
        {
            player.NoSamplesRead += () => AudioPlayerPool.Return(player);
            return player;
        }

        /// <summary>
        /// Disposes of the given resource when the <see cref="AudioPlayer"/> is destroyed or disabled.
        /// </summary>
        /// <param name="disposable">The resource to dispose of.</param>
        /// <returns>The player itself.</returns>
        [Obsolete("Prefer using audio processors instead.", true)]
        public AudioPlayer DisposeOnDestroy(IDisposable disposable)
        {
            player.Destroyed += disposable.Dispose;
            return player;
        }

    }

}
