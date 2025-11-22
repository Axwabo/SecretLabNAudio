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

        public AudioPlayer UseFile(string path) => player.Use(CreateAudioProcessor.FromFile(path));

        public AudioPlayer UseFile(string path, Action<ProcessorChain> process)
        {
            var chain = player.UseFile(path).ImmediateProviderAs<IAudioProcessor>()!.ToChain();
            process(chain);
            return player;
        }

    }

}
