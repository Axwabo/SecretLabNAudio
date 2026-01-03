using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Extensions.Providers;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <inheritdoc cref="ProviderMapper"/>
/// <typeparam name="T">The type of provider that will be returned.</typeparam>
public delegate T ProviderMapper<out T>(ISampleProvider current) where T : ISampleProvider;

/// <summary>
/// Extension methods for <see cref="ProcessorChain"/>s.
/// </summary>
public static class ProcessorChainExtensions
{

    /// <param name="mapper">The mapper to convert.</param>
    /// <typeparam name="T">The type of mapper.</typeparam>
    extension<T>(ProviderMapper<T> mapper) where T : ISampleProvider
    {

        private ProviderMapper AsNonGeneric => current => mapper(current);

    }

    /// <param name="chain">The audio processor chain.</param>
    extension(ProcessorChain chain)
    {

        /// <summary>
        /// Calls <see cref="ProcessorChain.Pop"/> before <see cref="ProcessorChain.Layer">layering</see> a new provider.
        /// </summary>
        /// <inheritdoc cref="ProcessorChain.Layer"/>
        public ProcessorChain Swap(ProviderMapper mapper, bool isOwned = true)
            => chain.Pop().Layer(mapper, isOwned);

        /// <summary>
        /// Resamples the chain to the given sample rate if needed.
        /// </summary>
        /// <param name="sampleRate">The target sample rate.</param>
        /// <returns>The chain itself.</returns>
        /// <remarks>The <see cref="ProcessorChain.Master"/> <see cref="WdlResamplingSampleProvider"/> will be swapped if needed.</remarks>
        public ProcessorChain Resample(int sampleRate)
            => chain.Master.WaveFormat.SampleRate == sampleRate
                ? chain
                : chain.SwapTOrLayer<WdlResamplingSampleProvider>(provider => provider.WaveFormat.SampleRate == sampleRate
                    ? provider
                    : new WdlResamplingSampleProvider(provider, sampleRate)
                );

        /// <summary>
        /// Mixes down the chain to mono if needed.
        /// </summary>
        /// <returns>The chain itself.</returns>
        /// <remarks>The <see cref="ProcessorChain.Master"/> <see cref="MonoToStereoSampleProvider"/> will be removed if needed.</remarks>
        public ProcessorChain ToMono() => chain.SwapTOrLayer<MonoToStereoSampleProvider>(static provider => provider.ToMono());

        /// <summary>
        /// Converts the chain to stereo if needed.
        /// </summary>
        /// <returns>The chain itself.</returns>
        /// <remarks>The <see cref="ProcessorChain.Master"/> <see cref="StereoToMonoSampleProvider"/> will be removed if needed.</remarks>
        public ProcessorChain ToStereo() => chain.SwapTOrLayer<StereoToMonoSampleProvider>(static provider => provider.ToStereo());

        /// <summary>
        /// Processes the chain to ensure that the <see cref="ProcessorChain.WaveFormat"/> matches <see cref="AudioPlayer.SupportedFormat"/>.
        /// </summary>
        /// <returns>The chain itself.</returns>
        /// <remarks>
        /// When mixing down is needed, the <see cref="ProcessorChain.Master"/> <see cref="WdlResamplingSampleProvider"/> will be removed.
        /// Then, the chain is converted to mono, after which a new resampler is added.
        /// </remarks>
        public ProcessorChain ToPlayerCompatible()
        {
            var format = chain.Master.WaveFormat;
            return (format.SampleRate == AudioPlayer.SampleRate, format.Channels == AudioPlayer.Channels) switch
            {
                (true, true) => chain,
                (false, false) => chain.Pop<WdlResamplingSampleProvider>().ToMono().Resample(AudioPlayer.SampleRate),
                (true, false) => chain.ToMono(),
                (false, true) => chain.Resample(AudioPlayer.SampleRate)
            };
        }

        /// <summary>
        /// Processes the chain to ensure that the <see cref="ProcessorChain.WaveFormat"/> matches the given sample rate and channel layout.
        /// </summary>
        /// <param name="sampleRate">The sample rate to convert to.</param>
        /// <param name="channels">The channel layout to convert to (1 for mono, 2 for stereo)</param>
        /// <returns>The chain itself.</returns>
        /// <remarks>
        /// This method improves performance in some cases compared to chaining <see cref="Resample"/> with <see cref="ToStereo"/>/<see cref="ToMono"/>.
        /// <p>
        /// When resampling and downmixing is needed, the <see cref="ProcessorChain.Master"/> <see cref="WdlResamplingSampleProvider"/> will be removed,
        /// then the chain will be mixed down to mono, then a new resampler is added.
        /// </p>
        /// <p>
        /// When resampling and converting to stereo is needed, the <see cref="ProcessorChain.Master"/> <see cref="MonoToStereoSampleProvider"/>
        /// is removed, then a resampler is added, then the chain is converted to stereo. 
        /// </p>
        /// </remarks>
        public ProcessorChain ToFormat(int sampleRate, int channels)
        {
            var format = chain.Master.WaveFormat;
            return (format.SampleRate == sampleRate, format.Channels == channels) switch
            {
                (true, true) => chain,
                (false, false) when channels == 1 => chain.Pop<WdlResamplingSampleProvider>().ToMono().Resample(sampleRate),
                (false, false) => chain.Pop<MonoToStereoSampleProvider>().Resample(sampleRate).ToStereo(),
                (true, false) when channels == 1 => chain.ToMono(),
                (true, false) => chain.ToStereo(),
                (false, true) => chain.Resample(sampleRate)
            };
        }

        /// <summary>
        /// Buffers the chain so it reads ahead by the given capacity.
        /// </summary>
        /// <param name="seconds">The number of seconds to read ahead.</param>
        /// <returns>The chain itself.</returns>
        /// <seealso cref="BufferedSampleProvider"/>
        public ProcessorChain Buffer(double seconds) => chain.SwapTOrLayer<BufferedSampleProvider>(provider => new BufferedSampleProvider(provider, seconds));

        /// <summary>
        /// Sets the master volume of the chain.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The chain itself.</returns>
        public ProcessorChain Volume(float volume = 1)
        {
            if (chain.Master is not VolumeSampleProvider volumeSampleProvider)
                return chain.Layer(provider => NonProcessorExtensions.Volume(provider, volume));
            volumeSampleProvider.Volume = volume;
            return chain;
        }

    }

    /// <param name="chain">The audio processor chain.</param>
    /// <typeparam name="T">The type to check for.</typeparam>
    extension<T>(ProcessorChain chain)
    {

        /// <summary>
        /// Pops the master layer if it's of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The chain itself.</returns>
        public ProcessorChain Pop() => chain.Master is T ? chain.Pop() : chain;

        /// <summary>
        /// Swaps the master layer if it's of type <typeparamref name="T"/>, adds a new layer otherwise.
        /// </summary>
        /// <param name="mapper">The </param>
        /// <param name="isOwned">Whether the new layer should be disposed when chain is disposed or if the layer is removed.</param>
        /// <returns>The chain itself.</returns>
        /// <seealso cref="Swap"/>
        /// <seealso cref="ProcessorChain.Layer"/>
        public ProcessorChain SwapTOrLayer(ProviderMapper mapper, bool isOwned = true)
            => chain.Master is T
                ? chain.Swap(mapper, isOwned)
                : chain.Layer(mapper, isOwned);

        /// <summary>
        /// Attempts to get the first layer whose provider is of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="layer">The layer if found, <see langword="null"/> otherwise.</param>
        /// <param name="provider">The provider if found, <see langword="null"/> otherwise.</param>
        /// <returns>Whether a layer was found.</returns>
        public bool TryGetLayer([NotNullWhen(true)] out ProcessorLayer? layer, [NotNullWhen(true)] out T? provider)
        {
            foreach (var processorLayer in chain.Layers)
            {
                if (processorLayer is not {Provider: T t})
                    continue;
                layer = processorLayer;
                provider = t;
                return true;
            }

            layer = null;
            provider = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the first provider of type <typeparamref name="T"/> from the list of layers.
        /// </summary>
        /// <param name="provider">The provider if found, <see langword="null"/> otherwise.</param>
        /// <returns>Whether a layer was found.</returns>
        public bool TryGetLayer([NotNullWhen(true)] out T? provider) => chain.TryGetLayer(out _, out provider);

    }

    /// <param name="chain">The audio processor chain to modify.</param>
    /// <typeparam name="T">The type of provider to map to.</typeparam>
    extension<T>(ProcessorChain chain) where T : ISampleProvider
    {

        /// <summary>
        /// Layers a new provider by mapping the <see cref="ProcessorChain.Master"/> and exports the created provider.
        /// </summary>
        /// <param name="mapper">The delegate to use to convert the provider.</param>
        /// <param name="provider">The newly created provider.</param>
        /// <param name="isOwned">Whether the new layer should be disposed when chain is disposed or if the layer is removed.</param>
        /// <returns>The chain itself.</returns>
        /// <seealso cref="ProcessorChain.Layer"/>
        public ProcessorChain Layer(ProviderMapper<T> mapper, out T provider, bool isOwned = true)
        {
            chain.Layer(mapper.AsNonGeneric, isOwned);
            provider = (T) chain.Master;
            return chain;
        }

        /// <summary>
        /// Swaps the master layer if it's of type <typeparamref name="T"/>, adds a new layer otherwise, and exports the created provider.
        /// </summary>
        /// <param name="mapper">The delegate to use to convert the provider.</param>
        /// <param name="provider">The newly created provider.</param>
        /// <param name="isOwned">Whether the new layer should be disposed when chain is disposed or if the layer is removed.</param>
        /// <returns>The chain itself.</returns>
        /// <seealso cref="SwapTOrLayer{T}(ProcessorChain,ProviderMapper,bool)"/>
        public ProcessorChain SwapTOrLayer(ProviderMapper<T> mapper, out T provider, bool isOwned = true)
        {
            chain.SwapTOrLayer<T>(mapper.AsNonGeneric, isOwned);
            provider = (T) chain.Master;
            return chain;
        }

    }

}
