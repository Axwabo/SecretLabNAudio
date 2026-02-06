using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>Extension methods for the <see cref="AudioQueue"/> class.</summary>
public static class AudioQueueExtensions
{

    /// <param name="queue">The queue to modify.</param>
    extension(AudioQueue queue)
    {

        /// <summary>
        /// Enqueues an anonymous file input.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The queue itself.</returns>
        /// <include file='../../XmlDocs/Files.xml' path='doc/exception'/>
        /// <include file='../../XmlDocs/Queue.xml' path='doc/EnqueueProcess/exception'/>
        public AudioQueue EnqueueFile(string path, ModifyChain? process = null)
            => queue.Enqueue(StreamAudioProcessor.CreateFromFile(path).Process(process));

        /// <summary>
        /// Attempts to enqueue an anonymous file input, avoiding nonexistent or unreadable files.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The queue itself.</returns>
        /// <include file='../../XmlDocs/Queue.xml' path='doc/EnqueueProcess/exception'/>
        /// <remarks>Nothing happens if the processor couldn't be created.</remarks>
        /// <seealso cref="StreamProcessorExtensions.TryCreateFromFile"/>
        public AudioQueue TryEnqueueFile(string path, ModifyChain? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, false, out var processor)
                ? queue.Enqueue(processor.Process(process))
                : queue;

        /// <summary>
        /// Enqueues an anonymous short clip input.
        /// </summary>
        /// <param name="name">The name of the short clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The queue itself.</returns>
        /// <include file='../../XmlDocs/Queue.xml' path='doc/EnqueueClip/exception'/>
        /// <include file='../../XmlDocs/Clips.xml' path='doc/TryGet/seealso'/>
        public AudioQueue EnqueueShortClip(ClipName name, ModifyChain? process = null)
            => ShortClipCache.TryGet(name, out var provider)
                ? queue.Enqueue(provider.Process(process), false)
                : queue;

    }

}
