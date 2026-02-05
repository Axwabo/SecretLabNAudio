using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class SpeakerToyExtensions
{

    extension(SpeakerToy speaker)
    {

        internal bool TryGetGroup([NotNullWhen(true)] out GroupedSpeaker? grouped) => speaker.Base.TryGetComponent(out grouped);

        public SpeakerToyGroup? Group => speaker.TryGetGroup(out var grouped) ? grouped.Group : null;

        public bool IsGrouped => speaker.TryGetGroup(out _);

        public bool IsGroupController => speaker.Group?.Controller == speaker;

        public bool IsGroupChild => speaker.Group?.IsChild(speaker) ?? false;

        public SpeakerToyGroup GetOrCreateGroup() => speaker.Group ?? speaker.CreateGroup();

        public SpeakerToyGroup CreateGroup()
        {
            if (speaker.IsGrouped)
                throw new InvalidOperationException("Speaker is already part of a group");
            var group = new SpeakerToyGroup(speaker);
            speaker.GameObject.AddComponent<GroupedSpeaker>().Group = group;
            return group;
        }

        public SpeakerToy CreateGroup(Action<SpeakerToyGroup> configure)
        {
            configure(speaker.CreateGroup());
            return speaker;
        }

    }

}
