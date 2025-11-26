namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioQueue? Queue => player.SingleInputAs<AudioQueue>();

        public Mixer? Mixer => player.ImmediateProviderAs<Mixer>();

    }

}
