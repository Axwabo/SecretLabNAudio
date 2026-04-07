using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class SpeakerToyExtensions
{

    extension(SpeakerToy speaker)
    {

        internal bool TryGetGroup([NotNullWhen(true)] out GroupedSpeaker? grouped)
        {
            if (!speaker.IsDestroyed)
                return speaker.Base.TryGetComponent(out grouped);
            grouped = null;
            return false;
        }

        internal void DestroyGroupTracker()
        {
            if (speaker.TryGetGroup(out var group))
                Object.Destroy(group);
        }

        /// <summary>
        /// Gets the current <see cref="SpeakerToyGroup"/> this speaker is part of, either as the controller, or as a child.
        /// </summary>
        public SpeakerToyGroup? Group => speaker.TryGetGroup(out var grouped) ? grouped.Group : null;

        /// <summary>
        /// Gets whether the speaker is part of a <see cref="SpeakerToyGroup"/>, either as the controller, or as a child.
        /// </summary>
        public bool IsGrouped => speaker.TryGetGroup(out _);

        /// <summary>
        /// Gets whether the speaker is the <see cref="SpeakerToyGroup.Controller"/> of its group (if it's grouped).
        /// </summary>
        public bool IsGroupController => speaker.Group?.Controller == speaker;

        /// <summary>
        /// Gets whether the speaker is a child of its <see cref="SpeakerToyGroup"/> (if it's grouped).
        /// </summary>
        public bool IsGroupChild => speaker.Group?.IsChild(speaker) ?? false;

        /// <summary>
        /// Gets the current <see cref="SpeakerToyGroup"/> of the speaker, or creates one if it isn't grouped.
        /// </summary>
        /// <returns>The existing or newly created group.</returns>
        /// <seealso cref="CreateGroup(SpeakerToy)"/>
        public SpeakerToyGroup GetOrCreateGroup() => speaker.Group ?? speaker.CreateGroup();

        /// <summary>
        /// Creates a new <see cref="SpeakerToyGroup"/> with this speaker as the controller.
        /// </summary>
        /// <returns>The created group.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the speaker is already part of a group.</exception>
        public SpeakerToyGroup CreateGroup()
        {
            if (speaker.IsGrouped)
                throw new InvalidOperationException("Speaker is already part of a group");
            var group = new SpeakerToyGroup(speaker);
            speaker.GameObject.AddComponent<GroupedSpeaker>().Group = group;
            return group;
        }

        /// <summary>
        /// Creates a new <see cref="SpeakerToyGroup"/> with this speaker as the controller.
        /// </summary>
        /// <param name="configure">An action to configure the group.</param>
        /// <returns>The speaker itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the speaker is already part of a group.</exception>
        public SpeakerToy CreateGroup(Action<SpeakerToyGroup> configure)
        {
            configure(speaker.CreateGroup());
            return speaker;
        }

    }

}
