namespace SecretLabNAudio.Core.Processors;

public interface IStreamAudioProcessor : IAudioProcessor, ISeekable, ILoopable
{

    string? FilePath { get; init; }

    long SamplePosition { get; set; }

}
