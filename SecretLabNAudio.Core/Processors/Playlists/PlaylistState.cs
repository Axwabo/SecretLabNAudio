namespace SecretLabNAudio.Core.Processors.Playlists;

/// <summary>
/// Represents the possible states of a <see cref="LazyPlaylist"/>.
/// </summary>
public enum PlaylistState
{

    /// <summary>
    /// The playlist has not started yet.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The current item is being played.
    /// </summary>
    PlayingIndex,

    /// <summary>
    /// The playlist is currently between items. Reading will start the next item if available.
    /// </summary>
    MovingToNextItem,

    /// <summary>
    /// The playlist has ended, reading will output nothing.
    /// Call <see cref="LazyPlaylist.RestartPlaylist"/> to restart the playlist.
    /// </summary>
    Ended

}
