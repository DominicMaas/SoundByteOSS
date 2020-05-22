using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    /// Extend custom service track classes
    /// off of this interface.
    /// </summary>
    public interface ITrack
    {
        /// <summary>
        ///     Convert the service specific track implementation to a
        ///     universal implementation. Overide this method and provide
        ///     the correct mapping between the service specific and universal
        ///     classes.
        /// </summary>
        /// <returns>A base track item.</returns>
        BaseTrack ToBaseTrack();

        /// <summary>
        ///     Likes a track.
        /// </summary>
        /// <returns>True is the track is liked, false if not.</returns>
        Task<bool> LikeAsync();

        /// <summary>
        ///     Unlikes a track.
        /// </summary>
        /// <returns>True if the track is not liked, false if it is.</returns>
        Task<bool> UnlikeAsync();
    }
}