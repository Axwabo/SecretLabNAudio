using System.Threading;
using CustomPlayerEffects;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Demo.Board;

public static class Outside
{

    private const float HueIncrement = 70 / 360f;

    private static CancellationToken _token = CancellationToken.None;

    private static readonly SpeakerSettings Settings = new()
    {
        Volume = 1,
        IsSpatial = true,
        MinDistance = 5,
        MaxDistance = 50
    };

    private static readonly SpeakerSettings Muted = Settings with {Volume = 0};

    private static readonly List<SpeakerPersonalization> PersonalizationInstances = [];

    public static void PlaceSpeakers(AudioPlayer controller)
    {
        var positions = Scp079InteractableBase.AllInstances
            .Where(e => e is Scp079Speaker {Room.Name: RoomName.Outside})
            .Select(e => e.Position);
        PersonalizationInstances.Clear();
        PersonalizationInstances.AddRange(controller.GetOrCreateGroup()
            .AddFromPool(Settings, positions)
            .AddPersonalizationToAll());
    }

    extension(Player p)
    {

        public bool IsOutside => p.Position.y > 250;

    }

    public static void RunEffects(CancellationToken cancellationToken)
    {
        // ReSharper disable once MergeIntoPattern
        if (_token.CanBeCanceled && !_token.IsCancellationRequested)
            return;
        _token = cancellationToken;
        _ = Effects(cancellationToken);
        _ = ManageMuting(cancellationToken);
    }

    private static async Awaitable Effects(CancellationToken cancellationToken)
    {
        var lights = Room.Get(RoomName.Outside).First().AllLightControllers.ToArray();
        var hue = 0f;
        while (!cancellationToken.IsCancellationRequested)
        {
            var color = Color.HSVToRGB(hue % 1, 0.6f, 0.5f);
            foreach (var light in lights)
                light.OverrideLightsColor = color;
            foreach (var player in Player.ReadyList)
                if (player.IsOutside)
                    player.EnableEffect<SoundtrackMute>();
                else
                    player.DisableEffect<SoundtrackMute>();
            hue += HueIncrement;
            await Awaitable.WaitForSecondsAsync(2, cancellationToken);
        }
    }

    private static async Awaitable ManageMuting(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Awaitable.WaitForSecondsAsync(0.5f, cancellationToken);
            var owner = DiscJockeyBoard.Instance!.Owner;
            if (owner == null)
                continue;
            if (DiscJockeyBoard.CanHearStageSpeaker(owner))
                MuteSpeakers(owner);
            else
                ClearMutes(owner);
        }
    }

    public static void MuteSpeakers(Player owner) => PersonalizationInstances.Override(owner, Muted);

    public static void ClearMutes(Player owner) => PersonalizationInstances.ClearOverride(owner);

}
