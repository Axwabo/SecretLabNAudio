using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

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

        public AudioPlayer UseFile(string path) => player.Use(CreateAudioProcessor.FromFile(path));

        public AudioPlayer UseFile(string path, Action<ProcessorChain> process)
        {
            var chain = CreateAudioProcessor.FromFile(path).ToChain();
            process(chain);
            return player.Use(chain);
        }

        public AudioPlayer UseQueue() => player.Use(new SampleProviderQueue(AudioPlayer.SupportedFormat));

        public AudioPlayer UseShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider)
                ? player.Use(loop ? provider.Loop() : provider)
                : player;

        public AudioPlayer UseExactShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider, false)
                ? player.Use(loop ? provider.Loop() : provider)
                : player;

    }

}
