using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

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
            if (player.SampleProvider is RawSourceSampleProvider raw)
                raw.Position = 0;
            else
                player.Stream?.Position = 0;
            return player;
        }

        public AudioPlayer Loop(bool loop = true)
        {
            if (player.SampleProvider is IAudioProcessor processor && processor.TryConvert(out StreamAudioProcessor? streamAudioProcessor))
                streamAudioProcessor.Loop = loop;
            return player;
        }

    }

}
