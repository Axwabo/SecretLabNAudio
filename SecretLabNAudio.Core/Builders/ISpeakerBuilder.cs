using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Groups;
using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Builders;

public interface ISpeakerBuilder
{

    SpeakerToyOutput Output { get; }

}

public static class SpeakerBuilderExtensions
{

    extension<T>(T builder) where T : ISpeakerBuilder
    {

        public SpeakerToy Speaker => builder.Output.Speaker;

        public T ApplySettings(SpeakerSettings settings)
        {
            builder.Speaker.ApplySettings(settings);
            return builder;
        }

        public T WithId(byte id)
        {
            builder.Speaker.ControllerId = id;
            return builder;
        }

        public T WithVolume(float volume)
        {
            builder.Speaker.Volume = volume;
            return builder;
        }

        public T WithMinDistance(float minDistance)
        {
            builder.Speaker.MinDistance = minDistance;
            return builder;
        }

        public T WithMaxDistance(float maxDistance)
        {
            builder.Speaker.MaxDistance = maxDistance;
            return builder;
        }

        public T WithSpatial(bool isSpatial = true)
        {
            builder.Speaker.IsSpatial = isSpatial;
            return builder;
        }

        public T PoolOnEnd()
        {
            builder.Output.PoolOnEnd = true;
            return builder;
        }

        public SpeakerToyGroup GetOrCreateGroup() => builder.Speaker.GetOrCreateGroup();

        public SpeakerToyGroup CreateGroup() => builder.Speaker.CreateGroup();

        public T CreateGroup(Action<SpeakerToyGroup> configure)
        {
            builder.Speaker.CreateGroup(configure);
            return builder;
        }

    }

}
