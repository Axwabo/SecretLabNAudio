namespace SecretLabNAudio.Core.Processors;

/// <summary>An interface specifying whether the resource can be looped.</summary>
public interface ILoopable
{

    /// <summary>Whether the provider should restart upon reaching the end.</summary>
    bool Loop { get; set; }

}
