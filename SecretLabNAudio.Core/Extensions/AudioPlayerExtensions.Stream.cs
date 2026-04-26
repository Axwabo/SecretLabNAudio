namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to modify the input of.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Sets the current time of the single <see cref="ISeekable"/> source to 0.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <remarks>Nothing will happen if there's more than 1 mixer input.</remarks>
        /// <seealso cref="SourceAs"/>
        public AudioPlayer Restart()
        {
            player.CurrentTime = TimeSpan.Zero;
            return player;
        }

        /// <summary>
        /// Gets or sets the current time of the single <see cref="ISeekable"/> source.
        /// </summary>
        /// <remarks>The value will be zero and will not be set if there's more than 1 mixer input.</remarks>
        /// <seealso cref="SourceAs"/>
        public TimeSpan CurrentTime
        {
            get => player.SourceAs<ISeekable>()?.CurrentTime ?? TimeSpan.Zero;
            set => player.SourceAs<ISeekable>()?.CurrentTime = value;
        }

        /// <summary>
        /// Gets the total time of the single <see cref="ISeekable"/> source.
        /// </summary>
        /// <remarks>The value will be zero if there's more than 1 mixer input.</remarks>
        /// <seealso cref="SourceAs"/>
        public TimeSpan TotalTime => player.SourceAs<ISeekable>()?.TotalTime ?? TimeSpan.Zero;

        /// <summary>
        /// Gets whether the single <see cref="ILoopable"/> source has its loop property set to true.
        /// </summary>
        /// <seealso cref="SourceAs"/>
        public bool IsLooping => player.SourceAs<ILoopable>()?.Loop ?? false;

        /// <summary>
        /// Sets whether to loop the single <see cref="ILoopable"/> input.
        /// </summary>
        /// <param name="loop">Whether to loop the provider.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>Nothing will happen if there's more than 1 mixer input.</remarks>
        /// <seealso cref="SourceAs"/>
        public AudioPlayer Loop(bool loop = true)
        {
            player.SourceAs<ILoopable>()?.Loop = loop;
            return player;
        }

    }

}
