using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public T? SourceAs<T>() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetSourceAs(out T? result) => result,
            _ => default
        };

        public T? MasterAs<T>() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetMasterAs(out T? result) => result,
            _ => default
        };

        public AudioQueue? Queue => player.SingleInputAs<AudioQueue>();

        public Mixer? Mixer => player.ImmediateProviderAs<Mixer>();

    }

}
