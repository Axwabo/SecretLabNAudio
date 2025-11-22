namespace SecretLabNAudio.Core.Processors;

public interface ILoopable
{

    /// <summary>Whether the provider should restart upon reaching the end.</summary>
    bool Loop { get; set; }

}
