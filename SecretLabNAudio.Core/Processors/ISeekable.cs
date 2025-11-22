namespace SecretLabNAudio.Core.Processors;

public interface ISeekable
{

    /// <summary>The current position of the provider as a <see cref="TimeSpan"/>.</summary>
    TimeSpan CurrentTime { get; set; }

    /// <summary>The duration of readable samples as a <see cref="TimeSpan"/>.</summary>
    TimeSpan TotalTime { get; }

}
