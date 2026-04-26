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

        /// <summary>
        /// Rents a <see cref="SpeakerToy"/> with an identical <see cref="SpeakerToy.ControllerId"/>, effectively cloning the output of this player elsewhere.
        /// The new speaker's settings will match that of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="position">The position to place the speaker at.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// The player's <see cref="SpeakerToyGroup"/> is used if possible.
        /// If the player isn't grouped, a new group will be created.
        /// </remarks>
        /// <seealso cref="GetOrCreateGroup"/>
        public AudioPlayer CloneOutput(Vector3 position)
            => player.CloneOutput(SpeakerSettings.From(player), position);

        /// <summary>
        /// Rents a <see cref="SpeakerToy"/> with an identical <see cref="SpeakerToy.ControllerId"/>, effectively cloning the output of this player elsewhere.
        /// </summary>
        /// <param name="settings">The settings to apply to the speaker.</param>
        /// <param name="position">The position to place the speaker at.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// The player's <see cref="SpeakerToyGroup"/> is used if possible.
        /// If the player isn't grouped, a new group will be created.
        /// </remarks>
        /// <seealso cref="GetOrCreateGroup"/>
        public AudioPlayer CloneOutput(SpeakerSettings settings, Vector3 position)
        {
            player.GetOrCreateGroup().AddFromPool(position, settings);
            return player;
        }

        /// <summary>
        /// Rents multiple <see cref="SpeakerToy"/>s with an identical <see cref="SpeakerToy.ControllerId"/>, effectively cloning the output of this player elsewhere.
        /// The new speakers' settings will match that of the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="positions">The positions to place speakers at.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// The player's <see cref="SpeakerToyGroup"/> is used if possible.
        /// If the player isn't grouped, a new group will be created.
        /// </remarks>
        /// <seealso cref="GetOrCreateGroup"/>
        public AudioPlayer CloneOutput(params IEnumerable<Vector3> positions)
            => player.CloneOutput(SpeakerSettings.From(player), positions);

        /// <summary>
        /// Rents multiple <see cref="SpeakerToy"/>s with an identical <see cref="SpeakerToy.ControllerId"/>, effectively cloning the output of this player elsewhere.
        /// </summary>
        /// <param name="settings">The settings to apply to the speakers.</param>
        /// <param name="positions">The positions to place speakers at.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// The player's <see cref="SpeakerToyGroup"/> is used if possible.
        /// If the player isn't grouped, a new group will be created.
        /// </remarks>
        /// <seealso cref="GetOrCreateGroup"/>
        public AudioPlayer CloneOutput(SpeakerSettings settings, params IEnumerable<Vector3> positions)
        {
            player.GetOrCreateGroup().AddFromPool(settings, positions);
            return player;
        }

    }

}
