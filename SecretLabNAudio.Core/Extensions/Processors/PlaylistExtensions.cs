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

        public PlaylistItem Amplify(float volume) => playlistItem.Process(ModifyChain.AmplifyIfNot1(volume));

    }

    extension(LazyPlaylist playlist)
    {

        public LazyPlaylist AddFile(string path, float volume = 1) => playlist.Add(new FilePlaylistItem(path).Amplify(volume));

        public LazyPlaylist AddShortClip(ClipName clipName, float volume = 1) => playlist.Add(new ShortClipPlaylistItem(clipName).Amplify(volume));

    }

}
