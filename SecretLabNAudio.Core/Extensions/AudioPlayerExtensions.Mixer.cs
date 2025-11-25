using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public T? SingleInputAs<T>() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSingleMixerInput(out T? result) => result,
            _ => default
        };

        public AudioPlayer MixAnonymous(ISampleProvider input, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddAnonymous(input, isOwned));

        public AudioPlayer MixNamed(ISampleProvider input, string inputName, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddNamed(input, inputName, isOwned));

        public AudioPlayer MixFileAnonymous(string path, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileAnonymous(path, loop));

        public AudioPlayer MixFileNamed(string path, string inputName, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileNamed(path, inputName, loop));

    }

}
