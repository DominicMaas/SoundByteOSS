using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.MusicProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Service used for managing tracks
    /// </summary>
    public interface ITrackService
    {
        /// <summary>
        ///     Resolve a track
        /// </summary>
        /// <param name="musicProvider">The music provider this track belongs to</param>
        /// <param name="id">The id of the track</param>
        /// <returns>The service track object</returns>
        Task<Track> ResolveTrackAsync(MusicProvider musicProvider, string id);

        /// <summary>
        ///     Resolve a list of tracks
        /// </summary>
        /// <param name="musicProvider">The music provider these tracks belong to</param>
        /// <param name="id">The ids of the tracks</param>
        /// <returns>A list of service track objects</returns>
        Task<IEnumerable<Track>> ResolveTracksAsync(MusicProvider musicProvider, string[] ids);

        /// <summary>
        ///     Like a track
        /// </summary>
        /// <param name="track">The track to like</param>
        Task LikeAsync(Track track);

        /// <summary>
        ///     Unlike a track
        /// </summary>
        /// <param name="track">The track to unlike</param>
        Task UnlikeAsync(Track track);

        /// <summary>
        ///     Check if the user has liked this track
        /// </summary>
        /// <param name="track">The track to check</param>
        /// <returns>If the user has liked this track</returns>
        Task<bool> HasLikedAsync(Track track);

        /// <summary>
        ///     Get the audio stream url
        /// </summary>
        /// <param name="track">The track to get the url for</param>
        /// <returns>The stream url</returns>
        Task<string> GetAudioStreamUrl(Track track);

        /// <summary>
        ///     Get the video stream url
        /// </summary>
        /// <param name="track">The track to get the url for</param>
        /// <returns>The stream url</returns>
        Task<string> GetVideoStreamUrl(Track track);
    }
}