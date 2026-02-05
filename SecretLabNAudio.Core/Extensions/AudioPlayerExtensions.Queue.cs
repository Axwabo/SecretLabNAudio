using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer Enqueue(ISampleProvider input, bool isOwned = true)
            => player.UseQueue(queue => queue.Enqueue(input, isOwned));

        public AudioPlayer EnqueueFile(string path, bool loop = false, float volume = 1)
            => player.EnqueueFile(path, ModifyChain.AmplifyIfNot1(volume), loop);

        public AudioPlayer EnqueueFile(string path, ModifyChain? process, bool loop = false)
            => player.UseQueue(queue => queue.EnqueueFile(path, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        public AudioPlayer EnqueueFileSafe(string path, bool loop = false, float volume = 1)
            => player.EnqueueFileSafe(path, ModifyChain.AmplifyIfNot1(volume), loop);

        public AudioPlayer EnqueueFileSafe(string path, ModifyChain? process, bool loop = false)
            => player.UseQueue(queue => queue.TryEnqueueFile(path, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        public AudioPlayer EnqueueShortClip(ClipName name, bool loop = false, float volume = 1)
            => player.UseQueue(queue => queue.EnqueueShortClip(name, loop, ModifyChain.AmplifyIfNot1(volume)));

    }

}
