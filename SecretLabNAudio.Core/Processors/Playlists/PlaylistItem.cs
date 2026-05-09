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

/// <summary>
/// A queued playlist item pointing to a file path.
/// </summary>
/// <param name="FilePath">The path to the file.</param>
public sealed record FilePlaylistItem(string FilePath) : PlaylistItem(Path.GetFileNameWithoutExtension(FilePath))
{

    /// <summary>
    /// Creates a sample provider for the playlist item.
    /// </summary>
    /// <param name="sampleRate">The sample rate of the playlist.</param>
    /// <param name="channels">The channel count of the playlist.</param>
    /// <returns>A new <see cref="StreamAudioProcessor"/> based on the item.</returns>
    /// <include file="../../XmlDocs/Files.xml" path="doc/exception"/>
    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
        => CreateAudioProcessor.FromFile(FilePath);

}

/// <summary>
/// A queued playlist item pointing to a <see cref="ClipName"/>, based on the <see cref="ShortClipCache"/>.
/// </summary>
/// <param name="ClipName">The name of the clip.</param>
public sealed record ShortClipPlaylistItem(ClipName ClipName) : PlaylistItem(ClipName.ToString())
{

    /// <summary>
    /// Creates a sample provider for the playlist item.
    /// </summary>
    /// <param name="sampleRate">The sample rate of the playlist.</param>
    /// <param name="channels">The channel count of the playlist.</param>
    /// <returns>A copy of the short clip in the cache.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no clip was added with the given key.</exception>
    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
        => ShortClipCache.Get(ClipName);

}

/// <summary>
/// A queued playlist item encapsulating another, modifying the provider using a <see cref="ProcessorChain"/>.
/// </summary>
/// <param name="Inner">The playlist item to use.</param>
/// <param name="Process">A delegate to process the provider.</param>
public sealed record ProcessedPlaylistItem(PlaylistItem Inner, ModifyChain Process) : PlaylistItem(Inner.Name)
{

    /// <summary>
    /// Creates a sample provider for the playlist item.
    /// </summary>
    /// <param name="sampleRate">The sample rate of the playlist.</param>
    /// <param name="channels">The channel count of the playlist.</param>
    /// <returns>A <see cref="ProcessorChain"/> based on the provider created by the <see cref="Inner"/> item.</returns>
    protected internal override ISampleProvider CreateProvider(int sampleRate, int channels)
    {
        var provider = Inner.CreateProvider(sampleRate, channels);
        var chain = ProviderToProcessor.SampleProviderToProcessor(provider, true).ToChain().ToFormat(sampleRate, channels);
        Process(chain);
        return chain;
    }

}
