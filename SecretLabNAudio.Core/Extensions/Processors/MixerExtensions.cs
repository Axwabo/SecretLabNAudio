using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class MixerExtensions
{

    extension(Mixer mixer)
    {

        public void RemoveAllByType<T>() where T : ISampleProvider => mixer.RemoveAll(static e => e.Provider is T);

        public void RemoveAllShortClips() => mixer.RemoveAllByType<RawSourceSampleProvider>();

    }

}
