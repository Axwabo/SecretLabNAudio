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

        /// <inheritdoc cref="CreateAudioProcessor.FromFile"/>
        public static StreamAudioProcessor CreateFromFile(string path, bool loop = false)
            => CreateAudioProcessor.FromFile(path).WithLoop(loop);

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
