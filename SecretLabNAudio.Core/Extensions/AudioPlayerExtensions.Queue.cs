using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        /// <summary>
        /// Enqueues a sample provider to be played.
        /// </summary>
        /// <param name="input">The provider to play from after the current one has ended.</param>
        /// <param name="isOwned">Whether to dispose of the input after it has ended or if the queue gets disposed.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueue.Enqueue(ISampleProvider,bool)"/>
        public AudioPlayer Enqueue(ISampleProvider input, bool isOwned = true)
            => player.UseQueue(queue => queue.Enqueue(input, isOwned));

        /// <summary>
        /// Enqueues a file to be played.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.EnqueueFile"/>
        public AudioPlayer EnqueueFile(string path, float volume = 1)
            => player.EnqueueFile(path, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Enqueues a file to be played.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.EnqueueFile"/>
        public AudioPlayer EnqueueFile(string path, ModifyChain? process)
            => player.UseQueue(queue => queue.EnqueueFile(path, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Attempts to enqueue a file to be played without throwing an exception if the file is not found or is not supported.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.TryEnqueueFile"/>
        public AudioPlayer EnqueueFileSafe(string path, float volume = 1)
            => player.EnqueueFileSafe(path, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Attempts to enqueue a file to be played without throwing an exception if the file is not found or is not supported.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.TryEnqueueFile"/>
        public AudioPlayer EnqueueFileSafe(string path, ModifyChain? process)
            => player.UseQueue(queue => queue.TryEnqueueFile(path, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Enqueues a short clip to be played (if a clip with the given name was registered).
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.EnqueueShortClip"/>
        public AudioPlayer EnqueueShortClip(ClipName name, float volume = 1)
            => player.EnqueueShortClip(name, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Enqueues a short clip to be played (if a clip with the given name was registered).
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Queue.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseQueue(AudioPlayer,Action{AudioQueue})"/>
        /// <seealso cref="AudioQueueExtensions.EnqueueShortClip"/>
        public AudioPlayer EnqueueShortClip(ClipName name, ModifyChain? process)
            => player.UseQueue(queue => queue.EnqueueShortClip(name, process));

    }

}
