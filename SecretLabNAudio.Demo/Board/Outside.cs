using System.Collections.Generic;
using System.Threading;
using CustomPlayerEffects;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079;
using SecretLabNAudio.Core.Pools;

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

    public static void PlaceSpeakers(byte id)
    {
        PersonalizationInstances.Clear();
        foreach (var interactable in Scp079InteractableBase.AllInstances)
            if (interactable is Scp079Speaker {Room.Name: RoomName.Outside})
                PersonalizationInstances.Add(SpeakerToyPool.Rent(null, interactable.Position)
                    .WithId(id)
                    .ApplySettings(Settings)
                    .AddPersonalization()
                );
    }

    public static bool IsOutside(this Player p) => p.Position.y > 250;

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
                if (player.IsOutside())
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

    public static void MuteSpeakers(Player owner)
    {
        foreach (var personalization in PersonalizationInstances)
            personalization.Override(owner, Muted);
    }

    public static void ClearMutes(Player owner)
    {
        foreach (var personalization in PersonalizationInstances)
            personalization.ClearOverride(owner);
    }

}
