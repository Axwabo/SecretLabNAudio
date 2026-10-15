using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Processors;

public interface IStreamAudioProcessor : IAudioProcessor, ISeekable, ILoopable
{

    long TotalSamples => WaveFormat.SampleCount(TotalTime.TotalSeconds);

    string? FilePath { get; init; }

}
