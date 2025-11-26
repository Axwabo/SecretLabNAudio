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

        public AudioPlayer Restart()
        {
            player.CurrentTime = TimeSpan.Zero;
            return player;
        }

        public AudioPlayer Loop(bool loop = true)
        {
            // TODO: mutate processor if needed
            player.SourceAs<ILoopable>()?.Loop = loop;
            return player;
        }

        public TimeSpan CurrentTime
        {
            get => player.SourceAs<ISeekable>()?.CurrentTime ?? TimeSpan.Zero;
            set => player.SourceAs<ISeekable>()?.CurrentTime = value;
        }

        public TimeSpan TotalTime => player.SourceAs<ISeekable>()?.TotalTime ?? TimeSpan.Zero;

        public bool IsLooping => player.SourceAs<ILoopable>()?.Loop ?? false;

    }

}
