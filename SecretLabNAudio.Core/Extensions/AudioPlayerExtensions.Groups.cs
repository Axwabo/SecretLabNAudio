using SecretLabNAudio.Core.Groups;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        internal bool TryGetGroup([NotNullWhen(true)] out GroupedSpeaker? grouped) => player.TryGetComponent(out grouped);

        public SpeakerToyGroup? Group => player.TryGetGroup(out var grouped) ? grouped.Group : null;

        public bool IsGrouped => player.TryGetGroup(out _);

        public bool IsGroupController => player.Group?.Controller == player.Speaker;

        public bool IsGroupChild => player.Group?.IsChild(player.Speaker) ?? false;

    }

}
