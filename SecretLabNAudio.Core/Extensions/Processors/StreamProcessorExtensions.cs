using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class StreamProcessorExtensions
{

    extension(StreamAudioProcessor processor)
    {

        public StreamAudioProcessor WithLoop(bool loop)
        {
            processor.Loop = loop;
            return processor;
        }

    }

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
