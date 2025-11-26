using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class MixerExtensions
{

    extension(Mixer mixer)
    {

        public Mixer AddFileAnonymous(string path, bool loop = false)
            => mixer.AddAnonymous(StreamAudioProcessor.CreateFromFile(path, loop));

        public Mixer AddFileNamed(string path, string inputName, bool loop = false)
            => mixer.AddNamed(StreamAudioProcessor.CreateFromFile(path, loop), inputName);

        public Mixer TryAddFileAnonymous(string path, bool loop = false)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddAnonymous(processor)
                : mixer;

        public Mixer TryAddFileNamed(string path, string inputName, bool loop = false)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddNamed(processor, inputName)
                : mixer;

        public Mixer RemoveAllByImmediateType<T>()
            => mixer.RemoveAll(static e => e.Provider is T);

        public Mixer RemoveAllBySourceType<T>()
            => mixer.RemoveAll(static e => e.Provider is T || e.Provider is IAudioProcessor provider && provider.TryGetSourceAs(out T? _));

        public Mixer RemoveAllShortClips()
        {
            mixer.RemoveAllByImmediateType<RawSourceSampleProvider>();
            mixer.RemoveAllByImmediateType<LoopingRawSampleProvider>();
            return mixer;
        }

        public Mixer RemoveAllStreamProcessors() => mixer.RemoveAllBySourceType<StreamAudioProcessor>();

        public Mixer RemoveAllByName(string name, bool ignoreCase = true)
            => mixer.RemoveAllByName(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    }

}
