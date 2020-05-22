using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Generic
{
    /// <summary>
    ///     An item that can be multiple things
    /// </summary>
    public class BaseSoundByteItem
    {
        public ItemType Type { get; set; }

        public BaseUser User { get; set; }

        public BasePlaylist Playlist { get; set; }

        public BaseTrack Track { get; set; }

        public BasePodcast Podcast { get; set; }

        public BaseSoundByteItem()
        {
            Type = ItemType.Unknown;
        }

        /// <summary>
        ///     Create an item for a user.
        /// </summary>
        /// <param name="user">The user</param>
        public BaseSoundByteItem(BaseUser user)
        {
            User = user;
            Type = ItemType.User;
        }

        /// <summary>
        ///     Create an item for a playlist.
        /// </summary>
        /// <param name="playlist">The playlist</param>
        public BaseSoundByteItem(BasePlaylist playlist)
        {
            Playlist = playlist;
            Type = ItemType.Playlist;
        }

        /// <summary>
        ///     Create an item for a track.
        /// </summary>
        /// <param name="track">The track</param>
        public BaseSoundByteItem(BaseTrack track)
        {
            Track = track;
            Type = ItemType.Track;
        }

        /// <summary>
        ///     Create an item for a podcast.
        /// </summary>
        /// <param name="podcast">The podcast</param>
        public BaseSoundByteItem(BasePodcast podcast)
        {
            Podcast = podcast;
            Type = ItemType.Podcast;
        }
    }
}