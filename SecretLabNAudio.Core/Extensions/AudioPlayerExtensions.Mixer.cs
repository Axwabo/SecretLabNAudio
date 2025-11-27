using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer Mix(ISampleProvider input, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddAnonymous(input, isOwned));

        public AudioPlayer Mix(ISampleProvider input, string inputName, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddNamed(input, inputName, isOwned));

        public AudioPlayer MixFile(string path, bool loop = false, float volume = 1)
            => player.MixFile(path, volume.AmplifyProcessorChain, loop);

        public AudioPlayer MixFile(string path, string inputName, bool loop = false, float volume = 1)
            => player.MixFile(path, inputName, volume.AmplifyProcessorChain, loop);

        public AudioPlayer MixFile(string path, Process? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileAnonymous(path, loop, process));

        public AudioPlayer MixFile(string path, string inputName, Process? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileNamed(path, inputName, loop, process));

        public AudioPlayer MixFileSafe(string path, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileAnonymous(path, loop));

        public AudioPlayer MixFileSafe(string path, string inputName, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileNamed(path, inputName, loop));

        public AudioPlayer MixShortClip(string name, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClip(name, loop));

        public AudioPlayer MixShortClip(string clipName, string inputName, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipNamed(clipName, inputName, loop));

        public AudioPlayer MixShortClipAnonymous(string name, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipAnonymous(name, loop));

    }

}
