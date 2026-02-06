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
        /// If the file exists and is supported, replaces the <see cref="AudioPlayer.SampleProvider"/> with a file stream processor.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer UseFileSafe(string path, bool loop = false, float volume = 1)
            => player.UseFileSafe(path, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// If the file exists and is supported, replaces the <see cref="AudioPlayer.SampleProvider"/> with a file stream processor. 
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="modify">A delegate to process the provider. If null, a <see cref="ProcessorChain"/> will only be created if format conversion is required.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer UseFileSafe(string path, ModifyChain? modify, bool loop = false)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? player.Use(processor.ToCompatibleProcessor().Process(modify))
                : player;

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

        /// <include file='../XmlDocs/Mixer.xml' path='doc/Use/summary'/>
        /// <param name="keepInputs">Whether to keep the current non-<see cref="Mixer"/> <see cref="AudioPlayer.SampleProvider"/> as an input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Use/remarks'/>
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

        /// <include file='../XmlDocs/Mixer.xml' path='doc/Use/summary'/>
        /// <param name="mix">A delegate to add inputs to the mixer with.</param>
        /// <param name="keepInputs">Whether to keep the current non-<see cref="Mixer"/> <see cref="AudioPlayer.SampleProvider"/> as an input.</param>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Use/remarks'/>
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
        /// <include file='../XmlDocs/Clips.xml' path='doc/TryGet/seealso'/>
        public AudioPlayer UseShortClip(ClipName name, bool loop = false, float volume = 1)
            => player.UseShortClip(name, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with a short clip (if a clip with the given name was registered).
        /// </summary>
        /// <param name="name">The name of the clip. The file extension is trimmed from the end.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Clips.xml' path='doc/TryGet/seealso'/>
        public AudioPlayer UseShortClip(ClipName name, ModifyChain? process, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider)
                ? player.WithUnmanagedProvider(provider.WithLoop(loop).Process(process))
                : player;

    }

}
