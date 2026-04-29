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

        public LazyPlaylist AddFile(string path, float volume = 1) => playlist.AddFile(path, ModifyChain.AmplifyIfNot1(volume));

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

        public LazyPlaylist AddShortClip(ClipName clipName, float volume = 1) => playlist.AddShortClip(clipName, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddShortClip(ClipName clipName, ModifyChain? process) => playlist.Add(new ShortClipPlaylistItem(clipName).Process(process));

        public LazyPlaylist AutoShuffle(bool shuffle = true)
        {
            playlist.ShuffleOnStart = shuffle;
            return playlist;
        }

        public LazyPlaylist RepeatAll()
        {
            playlist.RepeatMode = Repeat.All;
            return playlist;
        }

        public LazyPlaylist RepeatOne()
        {
            playlist.RepeatMode = Repeat.One;
            return playlist;
        }

        public LazyPlaylist NoRepeat()
        {
            playlist.RepeatMode = Repeat.None;
            return playlist;
        }

    }

}
