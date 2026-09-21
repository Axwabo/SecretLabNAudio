namespace SecretLabNAudio.Core.Builders;

public interface IAudioPlayerBuilder : IAudioPlayerBaseBuilder
{

    new AudioPlayer Player { get; }

    AudioPlayerBase IAudioPlayerBaseBuilder.Player => Player;

}

public static class AudioPlayerBuilderExtensions
{

    extension<T>(T builder) where T : IAudioPlayerBuilder
    {

        public T WithOutputMonitor(IAudioPacketMonitor monitor)
        {
            builder.Player.OutputMonitor = monitor;
            return builder;
        }

    }

}
