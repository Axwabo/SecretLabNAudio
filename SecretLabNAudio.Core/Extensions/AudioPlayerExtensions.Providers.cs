namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <summary>
    /// Safely casts the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="player">The player to cast the provider of.</param>
    /// <typeparam name="T">The type to cast the provider to.</typeparam>
    /// <returns>The <see cref="AudioPlayer.SampleProvider"/> cast to <typeparamref name="T"/>, or null if the type is not compatible.</returns>
    [Obsolete($"Call {nameof(ImmediateProviderAs)} to cast the {nameof(AudioPlayer.SampleProvider)} itself. To ignore special audio processors, use {nameof(RootAs)}/{nameof(MasterAs)}.", true)]
    public static T? ProviderAs<T>(this AudioPlayer player) where T : class => player.ImmediateProviderAs<T>();

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to set the provider of.</param>
    /// <param name="provider">The provider to set.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>This method ensures that the provider is compatible with the player by calling <see cref="SampleProviderExtensions.ToPlayerCompatible"/>.</remarks>
    [Obsolete($"Prefer using audio processors with the Use methods. Call {nameof(WithUnmanagedProvider)} to set the provider and prevent automatic disposal.", true)]
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
    [Obsolete("", true)] // TODO: add error message
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
    [Obsolete("", true)] // TODO: add error message
    public static AudioPlayer Buffer(this AudioPlayer player, double seconds)
    {
        if (player.SampleProvider == null)
            return player;
        player.SampleProvider = player.SampleProvider.Buffer(seconds);
        return player;
    }

    extension(AudioPlayer player)
    {

        /// <summary>
        /// Safely casts the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the provider to.</typeparam>
        /// <returns>The <see cref="AudioPlayer.SampleProvider"/> cast to <typeparamref name="T"/>, or null if the type is not compatible.</returns>
        public T? ImmediateProviderAs<T>() where T : class => player.SampleProvider as T;

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> and prevents automatic disposal when the player is destroyed or the provider changes.
        /// </summary>
        /// <param name="provider">The provider to set.</param>
        /// <returns>The <paramref name="player"/> itself.</returns>
        /// <remarks>
        /// Use this method if the provider is not disposable, or if you manage its lifetime yourself.
        /// This method ensures that the provider is compatible with the player by calling <see cref="SampleProviderExtensions.ToPlayerCompatible"/>.
        /// </remarks>
        public AudioPlayer WithUnmanagedProvider(ISampleProvider? provider)
        {
            player.SampleProvider = provider?.ToPlayerCompatible();
            player.OwnsProcessor = false;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> by converting an <see cref="IWaveProvider"/>
        /// and prevents automatic disposal when the player is destroyed or the provider changes.
        /// </summary>
        /// <param name="provider">The provider to set.</param>
        /// <returns>The <paramref name="player"/> itself.</returns>
        /// <remarks>
        /// Use this method if the provider is not disposable, or if you manage its lifetime yourself.
        /// This method ensures that the provider is compatible with the player by calling <see cref="WaveProviderExtensions.ToPlayerCompatible"/>.
        /// </remarks>
        /// <seealso cref="WithUnmanagedProvider(SecretLabNAudio.Core.AudioPlayer,NAudio.Wave.ISampleProvider?)"/>
        public AudioPlayer WithUnmanagedProvider(IWaveProvider? provider)
            => player.WithUnmanagedProvider(provider?.ToSampleProvider());

    }

}
