namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer Restart()
        {
            player.CurrentTime = TimeSpan.Zero;
            return player;
        }

        public AudioPlayer Loop(bool loop = true)
        {
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
