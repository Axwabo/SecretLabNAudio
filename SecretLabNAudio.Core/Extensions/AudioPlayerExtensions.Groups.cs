using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        internal bool TryGetGroup([NotNullWhen(true)] out GroupedSpeaker? grouped) => player.TryGetComponent(out grouped);

        public bool IsGrouped => player.TryGetGroup(out _);

        public bool IsGroupMaster => player.TryGetGroup(out var grouped) && grouped is SpeakerGroupMaster;

        public bool IsGroupChild => player.TryGetGroup(out var grouped) && grouped is not SpeakerGroupMaster;

        public

    }

}
