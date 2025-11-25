using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

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

        public static StreamAudioProcessor FromFile(string path, bool loop = false)
            => CreateAudioProcessor.FromFile(path).WithLoop(loop);

    }

}
