using SecretLabNAudio.Core.Extensions.Providers;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to set the provider of.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/>, and prevents automatic disposal when the player is destroyed or the provider changes.
        /// </summary>
        /// <param name="provider">The provider to set.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// Use this method if the provider is not disposable, or if you manage its lifetime yourself.
        /// This method ensures that the provider is compatible with the player by calling <see cref="NonProcessorExtensions.ToPlayerCompatible(ISampleProvider)"/>.
        /// </remarks>
        public AudioPlayer WithUnmanagedProvider(ISampleProvider? provider)
        {
            player.SampleProvider = provider?.ToPlayerCompatible();
            player.OwnsProvider = false;
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> by converting an <see cref="IWaveProvider"/>,
        /// and prevents automatic disposal when the player is destroyed or the provider changes.
        /// </summary>
        /// <param name="provider">The provider to set.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// Use this method if the provider is not disposable, or if you manage its lifetime yourself.
        /// This method ensures that the provider is compatible with the player by calling <see cref="NonProcessorExtensions.ToPlayerCompatible(IWaveProvider)"/>.
        /// </remarks>
        /// <seealso cref="WithUnmanagedProvider(SecretLabNAudio.Core.AudioPlayer,NAudio.Wave.ISampleProvider?)"/>
        public AudioPlayer WithUnmanagedProvider(IWaveProvider? provider)
            => player.WithUnmanagedProvider(provider?.ToSampleProvider());

    }

    /// <param name="player">The player to cast the provider of.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Safely casts the <see cref="AudioPlayer.SampleProvider"/> of the <see cref="AudioPlayer"/> to type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the provider to.</typeparam>
        /// <returns>The <see cref="AudioPlayer.SampleProvider"/> cast to <typeparamref name="T"/>, or null if the type is not compatible.</returns>
        public T? ImmediateProviderAs<T>() => player.SampleProvider is T t ? t : default;

    }

}
