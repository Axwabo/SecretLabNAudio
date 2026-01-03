using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <summary>
    /// Extension methods that modify the <see cref="AudioPlayer.SampleProvider"/>.
    /// </summary>
    /// <param name="player">The player to modify.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Converts the processor to be player-compatible, then sets the <see cref="AudioPlayer.SampleProvider"/> and <seealso cref="AudioPlayer.OwnsProvider"/> properties.
        /// </summary>
        /// <param name="processor">The processor to read from.</param>
        /// <param name="isOwned">Whether to dispose of the processor when the player is destroyed or pooled.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer Use(IAudioProcessor processor, bool isOwned = true)
        {
            player.SampleProvider = processor.ToPlayerCompatible(isOwned);
            player.OwnsProvider = isOwned;
            return player;
        }

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with a file stream processor.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer UseFile(string path, bool loop = false, float volume = 1)
            => player.UseFile(path, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with a file stream processor. 
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="modify">A delegate to process the provider. If null, a <see cref="ProcessorChain"/> will only be created if format conversion is required.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer UseFile(string path, ModifyChain? modify, bool loop = false)
            => player.Use(StreamAudioProcessor.CreateFromFile(path, loop).ToCompatibleProcessor().Process(modify));

        /// <summary>
        /// If the single input is not an <see cref="AudioQueue"/>, replaces the <see cref="AudioPlayer.SampleProvider"/> with a new one.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <seealso cref="SingleInputAs"/>
        public AudioPlayer UseQueue()
        {
            var queue = player.Queue;
            if (queue != null)
                return player;
            player.Use(new AudioQueue(AudioPlayer.SupportedFormat));
            return player;
        }

        /// <summary>
        /// If the single input is not an <see cref="AudioQueue"/>, replaces the <see cref="AudioPlayer.SampleProvider"/> with a new one.
        /// </summary>
        /// <param name="queue">A delegate to add items to the queue with.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer UseQueue(Action<AudioQueue> queue)
        {
            queue(player.UseQueue().Queue!);
            return player;
        }

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> to a <see cref="Mixer"/>.
        /// </summary>
        /// <param name="keepInputs">Whether to keep the current non-<see cref="Mixer"/> <see cref="AudioPlayer.SampleProvider"/> as an input.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// If <paramref name="keepInputs"/> is false, a new <see cref="Mixer"/> will be created.<br/>
        /// If <paramref name="keepInputs"/> is true, and the current provider is a <see cref="Mixer"/>, nothing happens.<br/>
        /// If <paramref name="keepInputs"/> is true, and the current provider is not a <see cref="Mixer"/>, the provider is set to a new mixer,
        /// and if the provider is set, the provider is added as an anonymous input with an ownership equivalent to <see cref="AudioPlayer.OwnsProvider"/>.
        /// </remarks>
        public AudioPlayer UseMixer(bool keepInputs = true)
        {
            if (!keepInputs)
                return player.Use(new Mixer(AudioPlayer.SupportedFormat));
            if (player.Mixer != null)
                return player;
            var mixer = new Mixer(AudioPlayer.SupportedFormat);
            if (player.SampleProvider is { } provider)
                mixer.AddAnonymous(provider, player.OwnsProvider);
            return player.Use(mixer);
        }

        /// <summary><inheritdoc cref="UseMixer(AudioPlayer,bool)" path="summary"/></summary>
        /// <param name="mix">A delegate to add inputs to the mixer with.</param>
        /// <param name="keepInputs">Whether to keep the current non-<see cref="Mixer"/> <see cref="AudioPlayer.SampleProvider"/> as an input.</param>
        /// <remarks><inheritdoc cref="UseMixer(AudioPlayer,bool)" path="remarks"/></remarks>
        public AudioPlayer UseMixer(Action<Mixer> mix, bool keepInputs = true)
        {
            mix(player.UseMixer(keepInputs).Mixer!);
            return player;
        }

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with a short clip (if a clip with the given name was registered).
        /// </summary>
        /// <param name="name">The name of the clip. The file extension is trimmed from the end.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="volume">The volume of the clip.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="UseExactShortClip"/>
        /// <seealso cref="ShortClipCache.TryGet"/>
        public AudioPlayer UseShortClip(string name, bool loop = false, float volume = 1)
            => player.UseShortClipHelper(name, true, loop, volume);

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with a short clip (if a clip with the given name was registered).
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="volume">The volume of the clip.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="UseExactShortClip"/>
        /// <seealso cref="ShortClipCache.TryGet"/>
        public AudioPlayer UseExactShortClip(string name, bool loop = false, float volume = 1)
            => player.UseShortClipHelper(name, false, loop, volume);

        private AudioPlayer UseShortClipHelper(string name, bool trimExtension, bool loop, float volume)
            => ShortClipCache.TryGet(name, out var provider, trimExtension)
                ? player.WithUnmanagedProvider(provider.WithLoop(loop).Process(ModifyChain.AmplifyIfNot1(volume)))
                : player;

    }

}
