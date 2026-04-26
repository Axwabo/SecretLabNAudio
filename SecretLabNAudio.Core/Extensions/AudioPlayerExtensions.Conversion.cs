using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to get the provider of.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Safely casts the single input to an <see cref="AudioQueue"/>.
        /// </summary>
        /// <remarks>
        /// The single input may be:
        /// <list type="number">
        /// <item>the immediate <see cref="AudioPlayer.SampleProvider"/></item>
        /// <item>the only input if the <see cref="AudioPlayer.SampleProvider"/> is a <see cref="Mixer"/></item>
        /// </list>
        /// </remarks>
        public AudioQueue? Queue => player.SingleInputAs<AudioQueue>();

        /// <summary>
        /// Safely casts the <see cref="AudioPlayer.SampleProvider"/> to a <see cref="Mixer"/>.
        /// </summary>
        public Mixer? Mixer => player.ImmediateProviderAs<Mixer>();

    }

    /// <param name="player">The player to get the provider of.</param>
    /// <typeparam name="T">The type to convert the provider to.</typeparam>
    extension<T>(AudioPlayer player)
    {

        /// <summary>
        /// Gets the source (original) provider of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The source provider of type <typeparamref name="T"/> if it was found, <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is of type <typeparamref name="T"/>, it will be returned.<br/>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is an <see cref="IAudioProcessor"/>, <see cref="AudioProcessorExtensions.TryGetSourceAs"/> will be called.
        /// </remarks>
        public T? SourceAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSourceAs(out T? result) => result,
            _ => default
        };

        /// <summary>
        /// Gets the master (final) provider of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The master provider of type <typeparamref name="T"/> if it was found, <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is of type <typeparamref name="T"/>, it will be returned.<br/>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is an <see cref="IAudioProcessor"/>, <see cref="AudioProcessorExtensions.TryGetMasterAs"/> will be called.
        /// </remarks>
        public T? MasterAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetMasterAs(out T? result) => result,
            _ => default
        };

        /// <summary>
        /// Gets the single mixer input of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The single input of type <typeparamref name="T"/> if it was found, <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is of type <typeparamref name="T"/>, it will be returned.<br/>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is an <see cref="IAudioProcessor"/>, <see cref="AudioProcessorExtensions.TryGetSingleMixerInput"/> will be called.
        /// </remarks>
        public T? SingleInputAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSingleMixerInput(out T? result) => result,
            _ => default
        };

    }

}
