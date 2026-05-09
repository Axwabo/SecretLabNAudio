namespace SecretLabNAudio.Core.Processors.Playlists;

/// <summary>
/// Represents values specifying looping modes in a <see cref="LazyPlaylist"/>.
/// </summary>
public enum Repeat
{

    /// <summary>
    /// Nothing is repeated.
    /// </summary>
    None,

    /// <summary>
    /// The current item is repeated.
    /// </summary>
    One,

    /// <summary>
    /// The whole playlist is repeated. When the last item ends, the first item will begin.
    /// </summary>
    All

}
