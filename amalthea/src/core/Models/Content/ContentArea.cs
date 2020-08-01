namespace SoundByte.Core.Models.Content
{
    /// <summary>
    ///     Where in the application should
    ///     this content group / section be shown
    /// </summary>
    public enum ContentArea
    {
        /// <summary>
        ///     Home tab / main entry point for the application
        /// </summary>
        Home,

        /// <summary>
        ///     Browse tab on the main view underneath the
        ///     tracks sub tab
        /// </summary>
        DiscoverMedia,

        /// <summary>
        ///     Podcasts tab on the main view underneath the
        ///     tracks sub tab
        /// </summary>
        DiscoverPodcasts,

        /// <summary>
        ///     Podcasts tab on the main view
        /// </summary>
        Podcasts,

        /// <summary>
        ///     My music tab on the main view underneath the
        ///     likes sub tab
        /// </summary>
        MyMusicLikes,

        /// <summary>
        ///     My music tab on the main view underneath the
        ///     playlists sub tab
        /// </summary>
        MyMusicPlaylists,

        /// <summary>
        ///     Tracks tab on the search page
        /// </summary>
        SearchTracks,

        /// <summary>
        ///     Playlists tab on the search page
        /// </summary>
        SearchPlaylists,

        /// <summary>
        ///     Users tab on the search page
        /// </summary>
        SearchUsers,

        /// <summary>
        ///     Podcasts tab on the search page
        /// </summary>
        SearchPodcasts,

        /// <summary>
        ///     Content to display underneath a user profile
        /// </summary>
        User
    }
}