using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// Extensions for <see cref="LazyPlaylist"/>s.
/// </summary>
public static class PlaylistExtensions
{

    extension(LazyPlaylist playlist)
    {

        /// <summary>
        /// Adds a cached file to the end of playlist.
        /// </summary>
        /// <param name="path">The path to the original file.</param>
        /// <param name="volume">The volume of the item.</param>
        /// <returns>The playlist itself.</returns>
        /// <include file="../XmlDocs/Playlist.xml" path="doc/remarks"/>
        public LazyPlaylist AddCachedFile(string path, float volume = 1)
            => playlist.AddCachedFile(path, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Adds a cached file to the end of playlist.
        /// </summary>
        /// <param name="path">The path to the original file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The playlist itself.</returns>
        /// <include file="../XmlDocs/Playlist.xml" path="doc/remarks"/>
        public LazyPlaylist AddCachedFile(string path, ModifyChain? process)
            => playlist.Add(new CachedFilePlaylistItem(path).Process(process));

        /// <summary>
        /// Adds multiple cached files to the end of the playlist.
        /// </summary>
        /// <param name="paths">Paths to the original files.</param>
        /// <returns>The playlist itself.</returns>
        /// <include file="../XmlDocs/Playlist.xml" path="doc/remarks"/>
        public LazyPlaylist AddCachedFiles(params IEnumerable<string> paths)
        {
            foreach (var path in paths)
                playlist.Add(new CachedFilePlaylistItem(path));
            return playlist;
        }

        /// <summary>
        /// Adds multiple cached files to the end of the playlist.
        /// </summary>
        /// <param name="paths">Paths to the original files.</param>
        /// <param name="volume">The volume of the items.</param>
        /// <returns>The playlist itself.</returns>
        /// <include file="../XmlDocs/Playlist.xml" path="doc/remarks"/>
        public LazyPlaylist AddCachedFiles(IEnumerable<string> paths, float volume)
            => playlist.AddCachedFiles(paths, ModifyChain.AmplifyIfNot1(volume));

        /// <summary>
        /// Adds multiple cached files to the end of the playlist.
        /// </summary>
        /// <param name="paths">Paths to the original files.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process streams.</param>
        /// <returns>The playlist itself.</returns>
        /// <include file="../XmlDocs/Playlist.xml" path="doc/remarks"/>
        public LazyPlaylist AddCachedFiles(IEnumerable<string> paths, ModifyChain? process)
        {
            foreach (var path in paths)
                playlist.AddCachedFile(path, process);
            return playlist;
        }

    }

}
