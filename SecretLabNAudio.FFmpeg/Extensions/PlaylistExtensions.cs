using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class PlaylistExtensions
{

    extension(LazyPlaylist playlist)
    {

        public LazyPlaylist AddCachedFile(string path, float volume = 1)
            => playlist.AddCachedFile(path, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddCachedFile(string path, ModifyChain? process)
            => playlist.Add(new CachedFilePlaylistItem(path).Process(process));

        public LazyPlaylist AddCachedFiles(params IEnumerable<string> paths)
        {
            foreach (var path in paths)
                playlist.Add(new CachedFilePlaylistItem(path));
            return playlist;
        }

        public LazyPlaylist AddCachedFiles(IEnumerable<string> paths, float volume)
            => playlist.AddCachedFiles(paths, ModifyChain.AmplifyIfNot1(volume));

        public LazyPlaylist AddCachedFiles(IEnumerable<string> paths, ModifyChain? process)
        {
            foreach (var path in paths)
                playlist.AddCachedFile(path, process);
            return playlist;
        }

    }

}
