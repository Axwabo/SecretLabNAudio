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
                : playlistItem is ProcessedPlaylistItem(var innerItem, var innerProcess)
                    ? new ProcessedPlaylistItem(innerItem, chain => process(innerProcess(chain)))
                    : new ProcessedPlaylistItem(playlistItem, process);

    }

    extension(LazyPlaylist playlist)
    {

        public LazyPlaylist AddFile(string path, float volume = 1) => playlist.AddFile(path, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddFile(string path, ModifyChain? process) => playlist.Add(new FilePlaylistItem(path).Process(process));

        public LazyPlaylist AddFileIfReadable(string path, float volume = 1) => playlist.AddFileIfReadable(path, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddFileIfReadable(string path, ModifyChain? process)
            => File.Exists(path) && AudioReaderFactoryManager.TryGetFactory(Path.GetExtension(path), out _)
                ? playlist.AddFile(path, process)
                : playlist;

        public LazyPlaylist AddShortClip(ClipName clipName, float volume = 1) => playlist.AddShortClip(clipName, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddShortClip(ClipName clipName, ModifyChain? process) => playlist.Add(new ShortClipPlaylistItem(clipName).Process(process));

        public LazyPlaylist AutoShuffle(bool shuffle = true)
        {
            playlist.ShuffleOnStart = shuffle;
            return playlist;
        }

    }

}
