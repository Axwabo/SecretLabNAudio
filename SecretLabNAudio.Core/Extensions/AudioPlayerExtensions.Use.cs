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

        public AudioPlayer UseFile(string path, bool loop = false) => player.Use(StreamAudioProcessor.FromFile(path, loop));

        public AudioPlayer UseFile(string path, Action<ProcessorChain> process, bool loop = false)
        {
            var chain = StreamAudioProcessor.FromFile(path, loop).ToChain();
            process(chain);
            return player.Use(chain);
        }

        public AudioPlayer UseQueue() => player.Use(new AudioQueue(AudioPlayer.SupportedFormat));

        public AudioPlayer UseMixer() => player.Use(new Mixer(AudioPlayer.SupportedFormat));

        public AudioPlayer UseMixer(Action<Mixer> mix)
        {
            mix(player.UseMixer().Mixer!);
            return player;
        }

        public AudioPlayer UseShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider)
                ? player.WithUnmanagedProvider(loop ? provider.Loop() : provider)
                : player;

        public AudioPlayer UseExactShortClip(string name, bool loop = false)
            => ShortClipCache.TryGet(name, out var provider, false)
                ? player.WithUnmanagedProvider(loop ? provider.Loop() : provider)
                : player;

    }

}
