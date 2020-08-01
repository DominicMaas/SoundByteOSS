namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     Media types within the application
    /// </summary>
    public enum MediaType
    {
        Unknown,
        Track,
        User,
        Playlist,

        /// <summary>
        ///     A podcast show (not an episode, similar to a playlist)
        /// </summary>
        PodcastShow,

        /// <summary>
        ///     An individual podcast episode
        /// </summary>
        PodcastEpisode
    }
}