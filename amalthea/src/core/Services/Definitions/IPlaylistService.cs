using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.MusicProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Used for managing user playlists, supports SoundByte
    ///     and music providers.
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        ///     Resolve a playlist
        /// </summary>
        /// <param name="musicProvider">The music provider this playlist belongs to</param>
        /// <param name="id">The id of the playlist</param>
        /// <returns>The service playlist object</returns>
        Task<Playlist> ResolvePlaylistAsync(MusicProvider musicProvider, string id);

        /// <summary>
        ///     Resolve a list of playlist
        /// </summary>
        /// <param name="musicProvider">The music provider these playlists belong to</param>
        /// <param name="id">The ids of the playlists</param>
        /// <returns>A list of service playlist objects</returns>
        Task<IEnumerable<Playlist>> ResolvePlaylistsAsync(MusicProvider musicProvider, string[] ids);

        /// <summary>
        ///     Get a full list of tracks in the playlist.
        /// </summary>
        /// <param name="playlist">The playlist to read</param>
        /// <returns>A list of tracks inside this playlist</returns>
        Task<IEnumerable<Track>> GetPlaylistTracksAsync(Playlist playlist);

        /// <summary>
        ///     Create a playlist using the provided information, returns
        ///     the created playlist (with correct platform id).
        /// </summary>
        /// <param name="playlist">Initial playlist information</param>
        /// <returns>A fully created playlist</returns>
        Task<Playlist> CreatePlaylistAsync(Playlist playlist);

        /// <summary>
        ///     Delete a playlist from the service.
        /// </summary>
        /// <param name="playlist">The playlist to delete</param>
        Task DeletePlaylistAsync(Playlist playlist);

        /// <summary>
        ///     Remove a track from the provided playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="track">The track to remove</param>
        Task AddTrackToPlaylistAsync(Playlist playlist, Track track);

        /// <summary>
        ///     Remove a track from the provided playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="track">The track to remove</param>
        Task RemoveTrackFromPlaylistAsync(Playlist playlist, Track track);
    }
}