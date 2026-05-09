namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed partial class LazyPlaylist
{

    public LazyPlaylist Add(PlaylistItem item)
    {
        _items.Add(item);
        return this;
    }

    public LazyPlaylist AddRange(params IEnumerable<PlaylistItem> items)
    {
        _items.AddRange(items);
        return this;
    }

    public LazyPlaylist Remove(PlaylistItem item)
    {
        var index = _items.IndexOf(item);
        return index == -1 ? this : RemoveAt(index);
    }

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

    public LazyPlaylist Clear()
    {
        _items.Clear();
        Index = 0;
        End(IsPlaying);
        return this;
    }

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

    public bool RestartCurrentItem()
    {
        if (GetSource<ISeekable>() is not { } seekable)
        {
            if (_current is not var (item, _) || !Begin(item, out _))
                return false;
            State = PlaylistState.PlayingIndex;
            return true;
        }

        seekable.CurrentTime = TimeSpan.Zero;
        return true;
    }

    public bool RestartPlaylist(bool? shuffle = null) => Restart(out _, shuffle);

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
