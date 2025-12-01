using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer Use(IAudioProcessor processor, bool isOwned = true)
        {
            player.SampleProvider = processor.ToPlayerCompatible(isOwned);
            return player.WithProviderOwnership(isOwned);
        }

        public AudioPlayer UseFile(string path, bool loop = false, float volume = 1)
            => player.Use(StreamAudioProcessor.CreateFromFile(path, loop).Process(volume.ModifyChainIfNot1));

        public AudioPlayer UseFile(string path, ModifyChain modify, bool loop = false)
            => player.Use(StreamAudioProcessor.CreateFromFile(path, loop).Process(modify));

        public AudioPlayer UseQueue()
        {
            var queue = new AudioQueue(AudioPlayer.SupportedFormat);
            if (player.SampleProvider is not Mixer mixer)
                return player.Use(queue);
            mixer.AddAnonymous(queue);
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
                mixer.AddAnonymous(provider, provider is IAudioProcessor);
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
                ? player.WithUnmanagedProvider(provider.WithLoop(loop).Process(volume.ModifyChainIfNot1))
                : player;

    }

}
