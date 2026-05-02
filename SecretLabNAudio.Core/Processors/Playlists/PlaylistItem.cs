using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Processors.Playlists;

/// <summary>
/// Represents a queued item in a <see cref="LazyPlaylist"/>.
/// </summary>
/// <param name="Name">The name of the item.</param>
public abstract record PlaylistItem(string? Name = null)
{

    /// <summary>
    /// Creates a sample provider for the playlist item.
    /// </summary>
    /// <param name="sampleRate">The sample rate of the playlist.</param>
    /// <param name="channels">The channel count of the playlist.</param>
    /// <returns>A new sample provider based on the item.</returns>
    /// <remarks>
    /// The <paramref name="sampleRate"/> and <paramref name="channels"/> parameters are hints only, allowing the inheritor to pass those to an external resource.
    /// <see cref="LazyPlaylist"/> will automatically convert the provider to its own format.
    /// </remarks>
    protected internal abstract ISampleProvider CreateProvider(int sampleRate, int channels);

}

public sealed record FilePlaylistItem(string FilePath) : PlaylistItem(Path.GetFileNameWithoutExtension(FilePath))
{

    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
        => CreateAudioProcessor.FromFile(FilePath);

}

public sealed record ShortClipPlaylistItem(ClipName ClipName) : PlaylistItem(ClipName.ToString())
{

    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
        => ShortClipCache.Get(ClipName);

}

public sealed record ProcessedPlaylistItem(PlaylistItem Inner, ModifyChain Process) : PlaylistItem(Inner.Name)
{

    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
    {
        var provider = Inner.CreateProvider(sampleRate, channels);
        var chain = ProviderToProcessor.SampleProviderToProcessor(provider, true).ToChain().ToFormat(sampleRate, channels);
        Process(chain);
        return chain;
    }

}
