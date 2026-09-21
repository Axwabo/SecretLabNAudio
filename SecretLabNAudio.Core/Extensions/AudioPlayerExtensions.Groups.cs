using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayerBase player)
    {

        /// <summary>
        /// Gets the current <see cref="SpeakerToyGroup"/> this player's speaker is part of, either as the controller, or as a child.
        /// </summary>
        public SpeakerToyGroup? Group => player.OutputSpeaker?.Group;

        /// <summary>
        /// Gets whether the player's speaker is part of a <see cref="SpeakerToyGroup"/>, either as the controller, or as a child.
        /// </summary>
        public bool IsGrouped => player.OutputSpeaker?.IsGrouped ?? false;

        /// <summary>
        /// Gets whether the player's speaker is the <see cref="SpeakerToyGroup.Controller"/> of its group (if it's grouped).
        /// </summary>
        public bool IsGroupController => player.OutputSpeaker?.IsGroupController ?? false;

        /// <summary>
        /// Gets whether the player's speaker is a child of its <see cref="SpeakerToyGroup"/> (if it's grouped).
        /// </summary>
        public bool IsGroupChild => player.OutputSpeaker?.IsGroupChild ?? false;

    }

}
