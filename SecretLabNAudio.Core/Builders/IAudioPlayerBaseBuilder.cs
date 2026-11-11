using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;
using SecretLabNAudio.Core.Pools;

// ReSharper disable InvokeAsExtensionMemberFromSameClass

namespace SecretLabNAudio.Core.Builders;

public interface IAudioPlayerBaseBuilder
{

    AudioPlayerBase Player { get; }

}

public static class AudioPlayerBaseBuilderExtensions
{

    extension<T>(T builder) where T : IAudioPlayerBaseBuilder
    {

        public T WithSendFilter(SendFilter filter)
        {
            builder.Player.SendFilter = filter;
            return builder;
        }

        public T WithSendFilter(Func<Player, bool> filter)
        {
            builder.Player.WithSendFilter(filter);
            return builder;
        }

        public T PoolOnEnd()
        {
            builder.Player.PoolOnEnd();
            return builder;
        }

        public T DestroyOnEnd()
        {
            builder.Player.DestroyOnEnd();
            return builder;
        }

        public T StopOnEnd()
        {
            builder.Player.StopOnEnd();
            return builder;
        }

    }

    extension<T>(IAudioPlayerBaseBuilder) where T : AudioPlayerBase
    {

        public static (T, SpeakerToyOutput) Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
            => Create<T>(SpeakerToyPool.NextAvailableId, settings, position, parent, spawn);

        public static (T, SpeakerToyOutput) Create(byte id, SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        {
            var speaker = SpeakerToy.Create(position, parent, false)
                .WithId(id)
                .ApplySettings(settings);
            var output = new SpeakerToyOutput(speaker);
            var o = speaker.GameObject;
            if (spawn)
                NetworkServer.Spawn(o);
            return (o.AddComponent<T>().WithPacketOutput(output), output);
        }

        public static (T, SpeakerToyOutput) CreatePrivate(Player target, float volume = 1, bool spawn = true)
        {
            var created = Create<T>(SpeakerSettings.GloballyAudible, Vector3.zero, target.GameObject!.transform, spawn);
            created.Item1.WithSendFilter(new SinglePlayerFilter(target));
            created.Item2.WithVolume(volume);
            return created;
        }

    }

}
