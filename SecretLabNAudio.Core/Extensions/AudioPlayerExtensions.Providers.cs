namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <summary>
    /// Safely casts the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="player">The player to cast the provider of.</param>
    /// <typeparam name="T">The type to cast the provider to.</typeparam>
    /// <returns>The <see cref="AudioPlayer.SampleProvider"/> cast to <typeparamref name="T"/>, or null if the type is not compatible.</returns>
    public static T? ProviderAs<T>(this AudioPlayer player) where T : class => player.SampleProvider as T;

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

}
