using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer Use(IAudioProcessor processor, bool isOwned = true)
        {
            player.SampleProvider = processor.ToPlayerCompatible(isOwned);
            player.OwnsProcessor = isOwned;
            return player;
        }

        public AudioPlayer UseFile(string path, bool loop = false) => player.Use(StreamAudioProcessor.CreateFromFile(path, loop));

        public AudioPlayer UseFile(string path, Action<ProcessorChain> process, bool loop = false)
        {
            var chain = StreamAudioProcessor.CreateFromFile(path, loop).ToChain();
            process(chain);
            return player.Use(chain);
        }

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
            var mixer = keepInputs && player.Mixer is { } existing ? existing : new Mixer(AudioPlayer.SupportedFormat);
            if (player.SampleProvider is not Mixer and { } provider)
                mixer.AddAnonymous(provider, provider is IAudioProcessor);
            return player.Use(mixer);
        }

        public AudioPlayer UseMixer(Action<Mixer> mix, bool keepInputs = true)
        {
            mix(player.UseMixer(keepInputs).Mixer!);
            return player;
        }

        public AudioPlayer UseShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider)
                ? player.WithUnmanagedProvider(provider.WithLoop(loop))
                : player;

        public AudioPlayer UseExactShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider, false)
                ? player.WithUnmanagedProvider(provider.WithLoop(loop))
                : player;

    }

}
