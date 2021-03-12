using System;
using System.Collections.Generic;

namespace SoundByte.Core
{
    /// <summary>
    ///     This class contains all the application keys
    /// </summary>
    public class AppKeys
    {
        #region SoundCloud

        public static readonly string SoundCloudClientId = "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT";

        public static readonly List<string> SoundCloudClientIds = new List<string>
        {
            "ytXCP8DpxZPd96FN12KsjT1P2mSHglXH",
            "59f81c512bd8eda616a21851093b2f16",
            "8547e755a4a625d4be8f243c1c7756a9",
            "0452ba585c12c2a37a143aca3b426b19"
        };

        #endregion SoundCloud

        #region Spotify

        public static readonly string SpotifyClientId = "7a5b9b286b6747cb828f9e9866067da7";

        #endregion Spotify

        #region YouTube

        public static readonly string YouTubeLoginClientId = "82331099881-4sf24bfns9f7p8b9o5ovvi8lveolru8i.apps.googleusercontent.com";
        public static readonly string YouTubeClientId = "AIzaSyACS4Bnh7dC1KOjx-BxBzChVb7_1vWQsVY";

        #endregion YouTube

        #region Generic

        public static readonly string AppLegacyCallback = Uri.EscapeUriString("http://localhost/soundbyte");

        #endregion Generic

        #region UWP

        public static readonly string GoogleAnalyticsUWPTrackerId = "UA-37972059-7";
        public static readonly string AppCenterUWPClientId = "792fe734-d9cc-4c13-8b00-6a8a180c598b";

        #endregion UWP
    }
}