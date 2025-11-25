using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class MixerExtensions
{

    private static bool MatchStreamed(MixerInput input) => input.Provider is IAudioProcessor processor && processor.TryGetSourceAs(out StreamAudioProcessor _);

    extension(Mixer mixer)
    {

        public void AddFileAnonymous(string path, bool loop = false)
            => mixer.AddAnonymous(StreamAudioProcessor.FromFile(path, loop));

        public void AddFileNamed(string path, string inputName, bool loop = false)
            => mixer.AddNamed(StreamAudioProcessor.FromFile(path, loop), inputName);

        public void RemoveAllByType<T>() where T : ISampleProvider
            => mixer.RemoveAll(static e => e.Provider is T);

        public void RemoveAllShortClips()
        {
            mixer.RemoveAllByType<RawSourceSampleProvider>();
            mixer.RemoveAllByType<LoopingRawSampleProvider>();
        }

        public void RemoveAllStreamed() => mixer.RemoveAll(MatchStreamed);

    }

}
