using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Builders;
using SecretLabNAudio.HighPerformance.Extensions;

namespace SecretLabNAudio.HighPerformance.Builders;

public interface IPreEncodedBuilder : IAudioPlayerBaseBuilder
{

    new PreEncodedAudioPlayer Player { get; }

    AudioPlayerBase IAudioPlayerBaseBuilder.Player => Player;

}

public static class PreEncodedBuilderExtensions
{

    extension<T>(T builder) where T : IPreEncodedBuilder
    {

        public T UseFile(string path, bool loop = false)
        {
            builder.Player.UseFile(path, loop);
            return builder;
        }

    }

}
