using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public bool TryGetStream([NotNullWhen(true)] out WaveStream? stream)
        {
            switch (player.SampleProvider)
            {
                case WaveStream waveStream:
                    stream = waveStream;
                    return true;
                case IAudioProcessor processor:
                    return processor.TryGetStream(out stream);
                default:
                    stream = null;
                    return false;
            }
        }

        public WaveStream? Stream => player.TryGetStream(out var stream) ? stream : null;

        public TimeSpan CurrentTime
        {
            get => player.Stream?.CurrentTime ?? TimeSpan.Zero;
            set => player.Stream?.CurrentTime = value;
        }

        public TimeSpan TotalTime => player.Stream?.TotalTime ?? TimeSpan.Zero;

        public AudioPlayer Restart()
        {
            player.Stream?.Position = 0;
            return player;
        }

    }

}
