namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed partial class LazyPlaylist
{

    public LazyPlaylist Add(PlaylistItem item)
    {
        _items.Add(item);
        return this;
    }

    public LazyPlaylist Remove(PlaylistItem item, bool stopCurrent = true)
    {
        var index = _items.IndexOf(item);
        return index == -1 ? this : RemoveAt(index, stopCurrent);
    }

    public LazyPlaylist RemoveAt(int index, bool stopCurrent = true)
    {
        _items.RemoveAt(index);
        if (index == Index)
        {
            if (stopCurrent)
                EndCurrent();
            if (IsPlaying)
                _isDetached = true;
            State = PlaylistState.BetweenItems;
        }
        else if (index < Index)
        {
            if (IsPlaying)
                _isDetached = true;
            Index--;
        }

        return this;
    }

    public LazyPlaylist Clear(bool stopCurrent = true)
    {
        _items.Clear();
        _isDetached = !stopCurrent;
        if (stopCurrent)
            End(IsPlaying);
        return this;
    }

    public bool Previous()
    {
        if (Index <= 0)
            return false;
        Index--;
        return _items.Count != 0 && Next(false, out _);
    }

    public bool Next()
    {
        if (!NextAvailable)
            return false;
        if (IsPlaying)
            return Next(true, out _);
        if (State == PlaylistState.NotStarted
            || State == PlaylistState.Ended && RepeatMode == Repeat.All
            || State == PlaylistState.BetweenItems && Index >= _items.Count - 1)
            return Restart(out _);
        return false;
    }

    public bool RestartCurrentItem()
    {
        if (GetSource<ISeekable>() is not { } seekable)
            return _current.HasValue && Next(false, out _);
        seekable.CurrentTime = TimeSpan.Zero;
        return true;
    }

    public bool RestartPlaylist(bool allowShuffle = true) => Restart(out _, allowShuffle);

    public void EndPlaylist() => End(IsPlaying);

    public bool TryGetCurrentTimestamps([NotNullWhen(true)] out TimeSpan? currentTime, [NotNullWhen(true)] out TimeSpan? totalTime)
    {
        var seekable = GetSource<ISeekable>();
        currentTime = seekable?.CurrentTime;
        totalTime = seekable?.TotalTime;
        return seekable != null;
    }

    public bool TrySeekCurrent(TimeSpan time)
    {
        if (GetSource<ISeekable>() is not { } seekable)
            return false;
        seekable.CurrentTime = time;
        return true;
    }

}
