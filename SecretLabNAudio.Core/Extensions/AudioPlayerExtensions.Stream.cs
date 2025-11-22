using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public T? RootAs<T>() => player.SampleProvider switch
        {
            T t => t,
            IAudioProcessor processor when processor.TryGetRootAs(out T? result) => result,
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
            player.RootAs<ILoopable>()?.Loop = loop;
            return player;
        }

        public TimeSpan CurrentTime
        {
            get => player.RootAs<ISeekable>()?.CurrentTime ?? TimeSpan.Zero;
            set => player.RootAs<ISeekable>()?.CurrentTime = value;
        }

        public TimeSpan TotalTime => player.RootAs<ISeekable>()?.TotalTime ?? TimeSpan.Zero;

        public bool IsLooping => player.RootAs<ILoopable>()?.Loop ?? false;

    }

}
