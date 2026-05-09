namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed partial class LazyPlaylist
{

    /// <summary>
    /// Adds an item at the end of the playlist.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The playlist itself.</returns>
    public LazyPlaylist Add(PlaylistItem item)
    {
        _items.Add(item);
        return this;
    }

    /// <summary>
    /// Adds multiple items at the end of the playlist.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <returns>The playlist itself.</returns>
    public LazyPlaylist AddRange(params IEnumerable<PlaylistItem> items)
    {
        _items.AddRange(items);
        return this;
    }

    /// <summary>
    /// Removes the first occurrence of the item.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>The playlist itself.</returns>
    public LazyPlaylist Remove(PlaylistItem item)
    {
        var index = _items.IndexOf(item);
        return index == -1 ? this : RemoveAt(index);
    }

    /// <summary>
    /// Remove the item at the specified index. If that is the current item, skips to the next one.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>The playlist itself.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0, or if <paramref name="index"/> is equal to or greater than the amount of items in the playlist.</exception>
    public LazyPlaylist RemoveAt(int index)
    {
        _items.RemoveAt(index);
        if (index == Index)
        {
            EndCurrent();
            if (IsPlaying)
                Next(false, out _);
        }
        else if (index < Index)
            Index--;

        return this;
    }

    /// <summary>
    /// Clears the playlist, stopping current playback (if any).
    /// </summary>
    /// <returns>The playlist itself.</returns>
    public LazyPlaylist Clear()
    {
        _items.Clear();
        Index = 0;
        End(IsPlaying);
        return this;
    }

    /// <summary>
    /// Skips to the previous item.
    /// </summary>
    /// <param name="wrapAround">
    /// If null, wrapping around will apply if <see cref="RepeatMode"/> is <see cref="Repeat.All"/>.
    /// When wrap around is enabled, the last item will be played if calling this method while the current item is the first item.
    /// </param>
    /// <returns>Whether the previous item was started.</returns>
    public bool Previous(bool? wrapAround = null)
    {
        if (_items.Count == 0)
            return false;
        if (Index > 0)
        {
            Index--;
            return Next(false, out _);
        }

        if (!wrapAround ?? RepeatMode != Repeat.All)
            return false;
        Index = _items.Count - 1;
        return Next(false, out _);
    }

    /// <summary>
    /// Skips to the next item.
    /// </summary>
    /// <param name="wrapAround">
    /// If null, wrapping around will apply if <see cref="RepeatMode"/> is <see cref="Repeat.All"/>.
    /// When wrap around is enabled, the first item will be played if calling this method while the current item is the last item.
    /// </param>
    /// <returns>Whether the next item was started.</returns>
    public bool Next(bool? wrapAround = null)
    {
        var wrap = wrapAround ?? RepeatMode == Repeat.All;
        if (!NextAvailable)
            return wrap && Restart(out _);
        if (IsPlaying)
            return Next(true, out _);
        if (State == PlaylistState.NotStarted || wrap && State == PlaylistState.Ended)
            return Restart(out _);
        return false;
    }

    /// <summary>
    /// Restarts the current item.
    /// </summary>
    /// <returns>Whether the current item was restarted.</returns>
    public bool RestartCurrentItem()
    {
        if (TrySeekCurrent(TimeSpan.Zero))
            return true;
        if (_current is not var (item, _) || !Begin(item, out _))
            return false;
        State = PlaylistState.PlayingIndex;
        return true;
    }

    /// <summary>
    /// Restarts the playlist as if it has not started yet. Plays the first item.
    /// </summary>
    /// <param name="shuffle">Whether to shuffle the playlist. If null, <see cref="ShuffleOnStart"/> is used to determine whether to shuffle.</param>
    /// <returns>Whether an item was started.</returns>
    public bool RestartPlaylist(bool? shuffle = null) => Restart(out _, shuffle);

    /// <summary>
    /// Ends playback and sets the <see cref="State"/> to <see cref="PlaylistState.Ended"/>.
    /// </summary>
    public void EndPlaylist() => End(IsPlaying);

    /// <summary>
    /// Attempts to get the timestamps of the current item.
    /// </summary>
    /// <param name="currentTime">The current time, if found.</param>
    /// <param name="totalTime">The total time, if found.</param>
    /// <returns>Whether the current item's source is <see cref="ISeekable"/>.</returns>
    public bool TryGetCurrentTimestamps([NotNullWhen(true)] out TimeSpan? currentTime, [NotNullWhen(true)] out TimeSpan? totalTime)
    {
        var seekable = GetSource<ISeekable>();
        currentTime = seekable?.CurrentTime;
        totalTime = seekable?.TotalTime;
        return seekable != null;
    }

    /// <summary>
    /// Attempts to seek the current item.
    /// </summary>
    /// <param name="time">The time to seek to.</param>
    /// <returns>Whether the current item's source is <see cref="ISeekable"/>.</returns>
    public bool TrySeekCurrent(TimeSpan time)
    {
        if (GetSource<ISeekable>() is not { } seekable)
            return false;
        seekable.CurrentTime = time;
        return true;
    }

}
