namespace SecretLabNAudio.Core;

/// <summary>An interface to process the output of an <see cref="AudioPlayer"/>.</summary>
public interface IAudioPacketMonitor
{

    /// <summary>
    /// Called when more than 0 samples were read from the <see cref="AudioPlayer.SampleProvider"/>.
    /// </summary>
    /// <param name="buffer">The samples that were read.</param>
    void OnRead(ReadOnlySpan<float> buffer);

    /// <summary>
    /// Called when the <see cref="AudioPlayer.SampleProvider"/> outputted no samples.
    /// </summary>
    void OnEmpty();

}
