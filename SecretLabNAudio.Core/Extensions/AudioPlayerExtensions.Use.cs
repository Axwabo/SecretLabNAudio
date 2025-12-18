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
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
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
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <seealso cref="ProcessorChainExtensions.ToPlayerCompatible"/>
        public AudioPlayer UseFile(string path, ModifyChain? modify, bool loop = false)
            => player.Use(StreamAudioProcessor.CreateFromFile(path, loop).ToCompatibleProcessor().Process(modify));

        public AudioPlayer UseQueue()
        {
            var queue = player.Queue;
            if (queue != null)
                return player;
            player.Use(new AudioQueue(AudioPlayer.SupportedFormat));
            return player;
        }

        public AudioPlayer UseQueue(Action<AudioQueue> queue)
        {
            queue(player.UseQueue().Queue!);
            return player;
        }

        public AudioPlayer UseMixer(bool keepInputs = true)
        {
            if (!keepInputs)
                return player.Use(new Mixer(AudioPlayer.SupportedFormat));
            if (player.Mixer != null)
                return player;
            var mixer = new Mixer(AudioPlayer.SupportedFormat);
            if (player.SampleProvider is not Mixer and { } provider)
                mixer.AddAnonymous(provider, player.OwnsProvider);
            return player.Use(mixer);
        }

        public AudioPlayer UseMixer(Action<Mixer> mix, bool keepInputs = true)
        {
            mix(player.UseMixer(keepInputs).Mixer!);
            return player;
        }

        public AudioPlayer UseShortClip(string name, bool loop = false, float volume = 1)
            => player.UseShortClipHelper(name, false, loop, volume);

        public AudioPlayer UseExactShortClip(string name, bool loop = false, float volume = 1)
            => player.UseShortClipHelper(name, false, loop, volume);

        private AudioPlayer UseShortClipHelper(string name, bool trimExtension, bool loop, float volume)
            => ShortClipCache.TryGet(name, out var provider, trimExtension)
                ? player.WithUnmanagedProvider(provider.WithLoop(loop).Process(ModifyChain.AmplifyIfNot1(volume)))
                : player;

    }

}
