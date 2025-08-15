using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="AudioPlayer"/> class.</summary>
public static class AudioPlayerExtensions
{

    private static AudioPlayer PatchSpeaker<T>(this AudioPlayer player, Func<SpeakerToy, T, SpeakerToy> method, T parameter)
    {
        method(player.Speaker, parameter);
        return player;
    }

    /// <summary>
    /// Applies the given <see cref="SpeakerSettings"/> to the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to apply the settings to.</param>
    /// <param name="settings">The settings to apply.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer ApplySettings(this AudioPlayer player, SpeakerSettings settings)
        => player.PatchSpeaker(SpeakerToyExtensions.ApplySettings, settings);

    /// <summary>
    /// Sets the controller ID of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the ID of.</param>
    /// <param name="id">The ID to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer WithId(this AudioPlayer player, byte id)
        => player.PatchSpeaker(SpeakerToyExtensions.WithId, id);

    /// <summary>
    /// Sets the volume of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the volume of.</param>
    /// <param name="volume">The volume to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>The volume must be in range 0-1. To further amplify the volume, use the <see cref="VolumeSampleProvider"/>.</remarks>
    public static AudioPlayer WithVolume(this AudioPlayer player, float volume)
        => player.PatchSpeaker(SpeakerToyExtensions.WithVolume, volume);

    /// <summary>
    /// Sets the minimum full volume distance of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the minimum distance of.</param>
    /// <param name="minDistance">The minimum distance to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <seealso cref="SpeakerSettings.MinDistance"/>
    public static AudioPlayer WithMinDistance(this AudioPlayer player, float minDistance)
        => player.PatchSpeaker(SpeakerToyExtensions.WithMinDistance, minDistance);

    /// <summary>
    /// Sets the maximum audible distance of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the maximum distance of.</param>
    /// <param name="maxDistance">The maximum distance to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <seealso cref="SpeakerSettings.MaxDistance"/>
    public static AudioPlayer WithMaxDistance(this AudioPlayer player, float maxDistance)
        => player.PatchSpeaker(SpeakerToyExtensions.WithMaxDistance, maxDistance);

    /// <summary>
    /// Sets whether the <see cref="AudioPlayer"/> is spatial (3D sound).
    /// /// </summary>
    /// <param name="player">The player to set the spatiality of.</param>
    /// <param name="isSpatial">Whether the player should be spatial.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer WithSpatial(this AudioPlayer player, bool isSpatial = true)
        => player.PatchSpeaker(SpeakerToyExtensions.WithSpatial, isSpatial);

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the provider of.</param>
    /// <param name="provider">The provider to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>This method ensures that the provider is compatible with the player by calling <see cref="SampleProviderExtensions.ToPlayerCompatible"/>.</remarks>
    public static AudioPlayer WithProvider(this AudioPlayer player, ISampleProvider? provider)
    {
        player.SampleProvider = provider?.ToPlayerCompatible();
        return player;
    }

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> by converting an <see cref="IWaveProvider"/>.
    /// </summary>
    /// <param name="player">The player to set the provider of.</param>
    /// <param name="provider">The provider to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <seealso cref="WithProvider(SecretLabNAudio.Core.AudioPlayer,NAudio.Wave.ISampleProvider?)"/>
    public static AudioPlayer WithProvider(this AudioPlayer player, IWaveProvider? provider)
        => player.WithProvider(provider?.ToSampleProvider());

    /// <summary>
    /// Sets the provider of the <see cref="AudioPlayer"/> to be a <see cref="Providers.BufferedSampleProvider"/>, reading ahead by <paramref name="seconds"/>.
    /// </summary>
    /// <param name="player">The player to buffer.</param>
    /// <param name="seconds">The number of seconds to buffer ahead.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>
    /// This method modifies the <see cref="AudioPlayer.SampleProvider"/>, therefore, changing the provider will remove buffering.
    /// If the current provider is null, no changes will be made.
    /// </remarks>
    /// <seealso cref="SampleProviderExtensions.Buffer"/>
    public static AudioPlayer Buffer(this AudioPlayer player, double seconds)
    {
        if (player.SampleProvider == null)
            return player;
        player.SampleProvider = player.SampleProvider.Buffer(seconds);
        return player;
    }

    /// <summary>
    /// Pauses (or unpauses) the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to pause.</param>
    /// <param name="pause">Whether to pause the player. Defaults to true.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer Pause(this AudioPlayer player, bool pause = true)
    {
        player.IsPaused = pause;
        return player;
    }

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SendEngine"/> of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the engine of.</param>
    /// <param name="engine">The engine to send audio with.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer WithSendEngine(this AudioPlayer player, SendEngine engine)
    {
        player.SendEngine = engine;
        return player;
    }

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SendEngine"/> of the <see cref="AudioPlayer"/> to a <see cref="FilteredSendEngine"/>.
    /// </summary>
    /// <param name="player">The player to set the engine of.</param>
    /// <param name="filter">The condition to satisfy for a <see cref="Player"/> to receive the audio.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer WithFilteredSendEngine(this AudioPlayer player, Predicate<Player> filter)
        => player.WithSendEngine(new FilteredSendEngine(filter));

    /// <summary>
    /// Sets the <see cref="AudioPlayer.OutputMonitor"/> of the <see cref="AudioPlayer"/> to the given <see cref="IAudioPacketMonitor"/>.
    /// </summary>
    /// <param name="player">The player to set the monitor of.</param>
    /// <param name="monitor">The monitor to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer WithOutputMonitor(this AudioPlayer player, IAudioPacketMonitor monitor)
    {
        player.OutputMonitor = monitor;
        return player;
    }

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SampleProvider"/> to <see langword="null"/> of the <see cref="AudioPlayer"/> when no samples are read.
    /// </summary>
    /// <param name="player">The player to unset the provider of.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer UnsetProviderOnEnd(this AudioPlayer player)
    {
        player.NoSamplesRead += () => player.SampleProvider = null;
        return player;
    }

    /// <summary>
    /// Destroys the <see cref="AudioPlayer"/> when no samples are read.
    /// </summary>
    /// <param name="player">The player to destroy.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer DestroyOnEnd(this AudioPlayer player)
    {
        player.UnsetProviderOnEnd();
        player.NoSamplesRead += player.Destroy;
        return player;
    }

    /// <summary>
    /// Returns the <see cref="AudioPlayer"/> to the pool when no samples are read.
    /// </summary>
    /// <param name="player">The player to return to the pool.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer PoolOnEnd(this AudioPlayer player)
    {
        player.NoSamplesRead += () => AudioPlayerPool.Return(player);
        return player;
    }

    /// <summary>
    /// Disposes of the given resource when the <see cref="AudioPlayer"/> is destroyed or disabled.
    /// </summary>
    /// <param name="player">The player that the resource should be disposed with.</param>
    /// <param name="disposable">The resource to dispose of.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    public static AudioPlayer DisposeOnDestroy(this AudioPlayer player, IDisposable disposable)
    {
        player.Destroyed += disposable.Dispose;
        return player;
    }

}
