using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors.Playlists;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class PlaylistExtensions
{

    extension(PlaylistItem playlistItem)
    {

        public PlaylistItem Process(ModifyChain? process)
            => process == null
                ? playlistItem
                : new ProcessedPlaylistItem(playlistItem, process);

    }

    extension(LazyPlaylist playlist)
    {

        /// <summary>
        /// Adds a file to the end of the playlist.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="volume">The volume of the item.</param>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist AddFile(string path, float volume = 1) => playlist.AddFile(path, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Adds a file to the end of the playlist.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist AddFile(string path, ModifyChain? process) => playlist.Add(new FilePlaylistItem(path).Process(process));

        public LazyPlaylist AddFileIfReadable(string path, float volume = 1) => playlist.AddFileIfReadable(path, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddFileIfReadable(string path, ModifyChain? process)
            => AudioReaderFactoryManager.IsReadable(path)
                ? playlist.AddFile(path, process)
                : playlist;

        public LazyPlaylist AddFiles(params IEnumerable<string> paths)
        {
            foreach (var path in paths)
                playlist.Add(new FilePlaylistItem(path));
            return playlist;
        }

        public LazyPlaylist AddFiles(IEnumerable<string> paths, float volume) => playlist.AddFiles(paths, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddFiles(IEnumerable<string> paths, ModifyChain? modifyChain)
        {
            foreach (var path in paths)
                playlist.AddFile(path, modifyChain);
            return playlist;
        }

        public LazyPlaylist AddReadableFiles(params IEnumerable<string> paths)
        {
            foreach (var path in paths)
                if (AudioReaderFactoryManager.IsReadable(path))
                    playlist.Add(new FilePlaylistItem(path));
            return playlist;
        }

        public LazyPlaylist AddReadableFiles(IEnumerable<string> paths, float volume) => playlist.AddFiles(paths, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddReadableFiles(IEnumerable<string> paths, ModifyChain? modifyChain)
        {
            foreach (var path in paths)
                playlist.AddFileIfReadable(path, modifyChain);
            return playlist;
        }

        /// <summary>
        /// Adds a short clip to the end of the playlist.
        /// </summary>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="volume">The volume of the item.</param>
        /// <returns>The playlist itself.</returns>
        /// <seealso cref="ShortClipCache"/>
        public LazyPlaylist AddShortClip(ClipName clipName, float volume = 1) => playlist.AddShortClip(clipName, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Adds a short clip to the end of the playlist.
        /// </summary>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The playlist itself.</returns>
        /// <seealso cref="ShortClipCache"/>
        public LazyPlaylist AddShortClip(ClipName clipName, ModifyChain? process) => playlist.Add(new ShortClipPlaylistItem(clipName).Process(process));

        /// <summary>
        /// Sets whether to shuffle items when the playlist (re)starts.
        /// </summary>
        /// <param name="shuffle">Whether to shuffle items.</param>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist AutoShuffle(bool shuffle = true)
        {
            playlist.ShuffleOnStart = shuffle;
            return playlist;
        }

        /// <summary>
        /// Sets <see cref="LazyPlaylist.RepeatMode"/> to <see cref="Repeat.All"/>.
        /// </summary>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist RepeatAll()
        {
            playlist.RepeatMode = Repeat.All;
            return playlist;
        }

        /// <summary>
        /// Sets <see cref="LazyPlaylist.RepeatMode"/> to <see cref="Repeat.One"/>.
        /// </summary>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist RepeatOne()
        {
            playlist.RepeatMode = Repeat.One;
            return playlist;
        }

        /// <summary>
        /// Sets <see cref="LazyPlaylist.RepeatMode"/> to <see cref="Repeat.None"/>.
        /// </summary>
        /// <returns>The playlist itself.</returns>
        public LazyPlaylist NoRepeat()
        {
            playlist.RepeatMode = Repeat.None;
            return playlist;
        }

    }

}
