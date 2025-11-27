using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioQueue? Queue => player.SingleInputAs<AudioQueue>();

        public Mixer? Mixer => player.ImmediateProviderAs<Mixer>();

    }

    extension<T>(AudioPlayer player)
    {

        public T? SourceAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSourceAs(out T? result) => result,
            _ => default
        };

        public T? MasterAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetMasterAs(out T? result) => result,
            _ => default
        };

        public T? SingleInputAs() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSingleMixerInput(out T? result) => result,
            _ => default
        };

    }

}
