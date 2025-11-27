using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class MixerExtensions
{

    extension(Mixer mixer)
    {

        public Mixer AddFileAnonymous(string path, bool loop = false, Process? process = null)
            => mixer.AddAnonymous(StreamAudioProcessor.CreateFromFile(path, loop).Process(process));

        public Mixer AddFileNamed(string path, string inputName, bool loop = false, Process? process = null)
            => mixer.AddNamed(StreamAudioProcessor.CreateFromFile(path, loop).Process(process), inputName);

        public Mixer AddShortClipAnonymous(string name, bool loop = false, Process? process = null)
            => ShortClipCache.TryGet(name, out var provider)
                ? mixer.AddAnonymous(provider.WithLoop(loop).Process(process), false)
                : mixer;

        public Mixer AddShortClip(string name, bool loop = false, Process? process = null)
            => ShortClipCache.TryGet(name, out var provider)
                ? mixer.AddNamed(provider.WithLoop(loop).Process(process), provider.ClipName!, false)
                : mixer;

        public Mixer AddShortClipNamed(string clipName, string inputName, bool loop = false, Process? process = null)
            => ShortClipCache.TryGet(clipName, out var provider)
                ? mixer.AddNamed(provider.WithLoop(loop).Process(process), inputName, false)
                : mixer;

        public Mixer TryAddFileAnonymous(string path, bool loop = false, Process? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddAnonymous(processor.Process(process))
                : mixer;

        public Mixer TryAddFileNamed(string path, string inputName, bool loop = false, Process? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddNamed(processor.Process(process), inputName)
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
