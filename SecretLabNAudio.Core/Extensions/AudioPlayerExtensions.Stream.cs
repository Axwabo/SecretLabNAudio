namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to modify the input of.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Sets the current time of the single <see cref="ISeekable"/> input to 0.
        /// </summary>
        /// <returns>The player itself.</returns>
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
