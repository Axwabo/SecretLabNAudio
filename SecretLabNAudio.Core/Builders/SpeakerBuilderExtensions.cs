using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Builders;

public static class SpeakerBuilderExtensions
{

    extension<T>(T builder) where T : ISpeakerBuilder
    {

        public T ApplySettings(SpeakerSettings settings)
        {
            builder.Speaker.ApplySettings(settings);
            return builder;
        }

    }

}
