using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Playback;
using YoutubeExplode;

namespace SoundByte.App.Uwp.ServicesV2
{
    public static class IPlaybackServiceEventHandlers
    {
        public delegate void TrackChangedEventHandler(BaseTrack newTrack);

        public delegate void StateChangedEventHandler(MediaPlaybackState mediaPlaybackState);
    }

    /// <summary>
    /// Handles background media playback in an app
    /// </summary>
    public interface IPlaybackService
    {
        #region Events

        event IPlaybackServiceEventHandlers.TrackChangedEventHandler OnTrackChange;

        event IPlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        #endregion Events

        #region Wrappers

        MediaPlayer GetMediaPlayer();

        MediaPlaybackList GetMediaPlaybackList();

        YoutubeClient GetYoutubeClient();

        #endregion Wrappers

        #region Shuffle Playlist

        /// <summary>
        ///     Returns whether the current playlist is
        ///     shuffled or not.
        /// </summary>
        /// <returns>Is the current playlist shuffled.</returns>
        bool IsPlaylistShuffled();

        /// <summary>
        ///     Shuffles or unshuffles the playlist depending on
        ///     the shuffle parameter.
        /// </summary>
        /// <param name="shuffle">If the playlist should be shuffled or not.</param>
        Task ShufflePlaylistAsync(bool shuffle);

        #endregion Shuffle Playlist

        #region Track Volume & Mute

        /// <summary>
        ///     Returns whether the current track is
        ///     muted or not.
        /// </summary>
        /// <returns>Is the current track muted.</returns>
        bool IsTrackMuted();

        /// <summary>
        ///     Mute or unmute the audio.
        /// </summary>
        /// <param name="mute">True if mute, false if not.</param>
        void MuteTrack(bool mute);

        /// <summary>
        ///     Returns the track volume.
        /// </summary>
        /// <returns>The track volume.</returns>
        double GetTrackVolume();

        /// <summary>
        ///     Sets the track volume.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        void SetTrackVolume(double volume);

        #endregion Track Volume & Mute

        #region Track Position

        /// <summary>
        ///     Returns the current track position throughout the song.
        /// </summary>
        /// <returns>The track position.</returns>
        TimeSpan GetTrackPosition();

        /// <summary>
        ///     Sets the track position.
        /// </summary>
        /// <param name="value">The position to set.</param>
        void SetTrackPosition(TimeSpan value);

        #endregion Track Position

        #region Track Repeat

        /// <summary>
        ///     Returns whether the track is
        ///     repeating or not.
        /// </summary>
        /// <returns>If the track is repeating.</returns>
        bool IsTrackRepeating();

        /// <summary>
        ///     Repeat or play as normal.
        /// </summary>
        /// <param name="repeat">True is repeated, false if not.</param>
        void RepeatTrack(bool repeat);

        #endregion Track Repeat

        #region Track Controls

        /// <summary>
        ///     Skip to the next track in
        ///     the playlist.
        /// </summary>
        void NextTrack();

        /// <summary>
        ///     Play the previous track in
        ///     the playlist.
        /// </summary>
        void PreviousTrack();

        /// <summary>
        ///     Pause the current track.
        /// </summary>
        void PauseTrack();

        /// <summary>
        ///     Plays the current track.
        /// </summary>
        void PlayTrack();

        /// <summary>
        ///     Move to the passed in track (only if the
        ///     track is in the current playlist).
        /// </summary>
        /// <param name="track">The track to move to.</param>
        void MoveToTrack(BaseTrack track);

        #endregion Track Controls

        #region Track Information

        /// <summary>
        ///     Get the duration of the track.
        /// </summary>
        /// <returns>The track duration.</returns>
        TimeSpan GetTrackDuration();

        /// <summary>
        ///     Get the current playback state of the track.
        /// </summary>
        /// <returns>The playback state of the track.</returns>
        MediaPlaybackState GetPlaybackState();

        /// <summary>
        ///     Get the current playing track (if any).
        ///     Will be null if no tracks are playing
        /// </summary>
        /// <returns>Returns the current playing track. .</returns>
        BaseTrack GetCurrentTrack();

        #endregion Track Information

        #region Playlist Information

        /// <summary>
        ///     Get the list of tracks currently in the playlist.
        /// </summary>
        /// <returns>A list of tracks in the playlist.</returns>
        List<BaseTrack> GetPlaylist();

        /// <summary>
        ///     Returns the current playlist token
        ///     (next items in the list).
        /// </summary>
        /// <returns>The playlist token.</returns>
        string GetPlaylistToken();

        /// <summary>
        ///     Returns the source which tells SoundByte
        ///     which items to load.
        /// </summary>
        /// <returns>The item loading source.</returns>
        ISource GetPlaylistSource();

        #endregion Playlist Information

        #region Start Track

        /// <summary>
        ///     Start playlist at a specific track.
        /// </summary>
        /// <param name="trackToPlay">The track to play, must exist in the playlist.</param>
        /// <param name="startTime">Time to start playing the track</param>
        Task StartTrackAsync(BaseTrack trackToPlay, TimeSpan? startTime = null);

        /// <summary>
        ///     Start playlist at a random track.
        /// </summary>
        Task StartRandomTrackAsync();

        #endregion Start Track

        #region Initialize Playlist

        /// <summary>
        ///     Setup a playlist and start playing music.
        /// </summary>
        /// <param name="model">The model to get more music.</param>
        /// <param name="playlist">List of items in the playlist to play.</param>
        /// <param name="token">Token to get more information.</param>
        /// <returns>The playback start response.</returns>
        Task<PlaybackInitializeResponse> InitializePlaylistAsync(ISource model, IEnumerable<BaseSoundByteItem> playlist = null, string token = null);

        #endregion Initialize Playlist
    }
}