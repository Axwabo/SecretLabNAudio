using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// Extension methods for the <see cref="StreamAudioProcessor"/> class.
/// </summary>
public static class StreamProcessorExtensions
{

    /// <param name="processor">The processor to modify.</param>
    extension(StreamAudioProcessor processor)
    {

        /// <summary>
        /// Sets the <see cref="StreamAudioProcessor.Loop"/> property.
        /// </summary>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>The processor itself.</returns>
        public StreamAudioProcessor WithLoop(bool loop)
        {
            processor.Loop = loop;
            return processor;
        }

    }

    /// <summary>Methods to create a <see cref="StreamAudioProcessor"/> from a file.</summary>
    extension(StreamAudioProcessor)
    {

        /// <summary>
        /// Creates a <see cref="StreamAudioProcessor"/> from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>An audio processor that reads from the file.</returns>
        /// <include file='../../XmlDocs/Files.xml' path='doc/exception'/>
        /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
        public static StreamAudioProcessor CreateFromFile(string path, bool loop = false)
            => CreateAudioProcessor.FromFile(path).WithLoop(loop);

        /// <summary>
        /// Attempts to create a <see cref="StreamAudioProcessor"/> from the given file path.
        /// </summary>
        /// <param name="path">The file path to read the audio from.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <param name="processor">
        /// The resulting <see cref="StreamAudioProcessor"/> if successful.
        /// <see langword="null"/> if the file was not found, if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.
        /// </param>
        /// <returns>Whether a <see cref="StreamAudioProcessor"/> was successfully created.</returns>
        /// <remarks><inheritdoc cref="CreateAudioProcessor.FromFile" path="remarks"/></remarks>
        public static bool TryCreateFromFile(string path, bool loop, [NotNullWhen(true)] out StreamAudioProcessor? processor)
        {
            if (!File.Exists(path) || !TryCreateAudioProcessor.FromFile(path, out var result))
            {
                processor = null;
                return false;
            }

            processor = result.WithLoop(loop);
            return true;
        }

    }

}
