namespace SecretLabNAudio.Core.Processors;

public interface ISeekable
{

    TimeSpan CurrentTime { get; set; }

    TimeSpan TotalTime { get; }

}
