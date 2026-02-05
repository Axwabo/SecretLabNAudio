using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        /// <summary>
        /// Gets the current <see cref="SpeakerToyGroup"/> this player's speaker is part of, either as the controller, or as a child.
        /// </summary>
        public SpeakerToyGroup? Group => player.Speaker.Group;

        /// <summary>
        /// Gets whether the player's speaker is part of a <see cref="SpeakerToyGroup"/>, either as the controller, or as a child.
        /// </summary>
        public bool IsGrouped => player.Speaker.IsGrouped;

        /// <summary>
        /// Gets whether the player's speaker is the <see cref="SpeakerToyGroup.Controller"/> of its group (if it's grouped).
        /// </summary>
        public bool IsGroupController => player.Speaker.IsGroupController;

        /// <summary>
        /// Gets whether the player's speaker is a child of its <see cref="SpeakerToyGroup"/> (if it's grouped).
        /// </summary>
        public bool IsGroupChild => player.Speaker.IsGroupChild;

        /// <summary>
        /// Gets the current <see cref="SpeakerToyGroup"/> of the player's speaker, or creates one if it isn't grouped.
        /// </summary>
        /// <returns>The existing or newly created group.</returns>
        /// <seealso cref="CreateGroup(AudioPlayer)"/>
        public SpeakerToyGroup GetOrCreateGroup() => player.Speaker.GetOrCreateGroup();

        /// <summary>
        /// Creates a new <see cref="SpeakerToyGroup"/> with this player's speaker as the controller.
        /// </summary>
        /// <returns>The created group.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the player's speaker is already part of a group.</exception>
        public SpeakerToyGroup CreateGroup() => player.Speaker.CreateGroup();

        /// <summary>
        /// Creates a new <see cref="SpeakerToyGroup"/> with this player's speaker as the controller.
        /// </summary>
        /// <param name="configure">An action to configure the group.</param>
        /// <returns>The player itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the player's speaker is already part of a group.</exception>
        public AudioPlayer CreateGroup(Action<SpeakerToyGroup> configure)
        {
            player.Speaker.CreateGroup(configure);
            return player;
        }

    }

}
