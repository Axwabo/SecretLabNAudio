using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// Extension methods for the <see cref="WaveStreamAudioProcessor"/> class.
/// </summary>
public static class StreamProcessorExtensions
{

    private const int BufferLength = 48000;

    /// <param name="processor">The processor to modify.</param>
    extension(IStreamAudioProcessor processor)
    {

        /// <summary>
        /// Sets the <see cref="WaveStreamAudioProcessor.Loop"/> property.
        /// </summary>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>The processor itself.</returns>
        public IStreamAudioProcessor WithLoop(bool loop)
        {
            processor.Loop = loop;
            return processor;
        }

    }

    /// <summary>Methods to create a <see cref="WaveStreamAudioProcessor"/> from a file.</summary>
    extension(IStreamAudioProcessor)
    {

        /// <summary>
        /// Creates a <see cref="WaveStreamAudioProcessor"/> from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>An audio processor that reads from the file.</returns>
        /// <include file="../../XmlDocs/Files.xml" path="doc/exception"/>
        /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
        public static IStreamAudioProcessor CreateFromFile(string path, bool loop = false)
            => CreateAudioProcessor.FromFile(path).WithLoop(loop);

        /// <summary>
        /// Attempts to create a <see cref="WaveStreamAudioProcessor"/> from the given file path.
        /// </summary>
        /// <param name="path">The file path to read the audio from.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <param name="processor">
        /// The resulting <see cref="WaveStreamAudioProcessor"/> if successful.
        /// <see langword="null"/> if the file was not found, if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.
        /// </param>
        /// <returns>Whether a <see cref="WaveStreamAudioProcessor"/> was successfully created.</returns>
        /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
        public static bool TryCreateFromFile(string path, bool loop, [NotNullWhen(true)] out IStreamAudioProcessor? processor)
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
