namespace SecretLabNAudio.Core.Processors;

/// <summary>An interface specifying a duration and a modifiable current time of the audio resource.</summary>
public interface ISeekable
{

    /// <summary>The current position of the provider as a <see cref="TimeSpan"/>.</summary>
    TimeSpan CurrentTime { get; set; }

    /// <summary>The duration of readable samples as a <see cref="TimeSpan"/>.</summary>
    TimeSpan TotalTime { get; }

}
