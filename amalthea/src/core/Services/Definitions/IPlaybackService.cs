using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Playback;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Handles background media playback in an app
    /// </summary>
    public interface IPlaybackService
    {
        #region Events

        event PlaybackServiceEventHandlers.MediaChangedEventHandler OnMediaChange;

        event PlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        #endregion Events

        #region Shuffle Queue

        /// <summary>
        ///     Returns whether the current queue is
        ///     shuffled or not.
        /// </summary>
        /// <returns>Is the current playlist shuffled.</returns>
        bool IsQueueShuffled();

        /// <summary>
        ///     Shuffles or unshuffles the queue depending on
        ///     the shuffle parameter.
        /// </summary>
        /// <param name="shuffle">If the queue should be shuffled or not.</param>
        Task ShuffleQueueAsync(bool shuffle);

        #endregion Shuffle Queue

        #region Media Volume & Mute

        /// <summary>
        ///     Returns whether the current media is
        ///     muted or not
        /// </summary>
        /// <returns>Is the current media muted</returns>
        bool IsMediaMuted();

        /// <summary>
        ///     Mute or unmute the audio
        /// </summary>
        /// <param name="mute">True if mute, false if not</param>
        void MuteMedia(bool mute);

        /// <summary>
        ///     Returns the media volume
        /// </summary>
        /// <returns>The media volume</returns>
        double GetMediaVolume();

        /// <summary>
        ///     Sets the media volume
        /// </summary>
        /// <param name="volume">The volume to set</param>
        void SetMediaVolume(double volume);

        #endregion Media Volume & Mute

        #region Media Position

        /// <summary>
        ///     Returns the current media position throughout the song
        /// </summary>
        /// <returns>The track position</returns>
        TimeSpan GetMediaPosition();

        /// <summary>
        ///     Sets the media position
        /// </summary>
        /// <param name="value">The position to set</param>
        void SetMediaPosition(TimeSpan value);

        #endregion Media Position

        #region Media Playback Rate

        /// <summary>
        ///     Returns the current media playback rate
        /// </summary>
        /// <returns>The current rate</returns>
        double GetMediaPlaybackRate();

        /// <summary>
        ///     Sets the media playback rate
        /// </summary>
        /// <param name="value">The rate to set</param>
        void SetMediaPlaybackRate(double value);

        #endregion Media Playback Rate

        #region Media Repeat

        /// <summary>
        ///     Returns whether the media is
        ///     repeating or not
        /// </summary>
        /// <returns>If the media is repeating</returns>
        bool IsMediaRepeating();

        /// <summary>
        ///     Repeat or play as normal
        /// </summary>
        /// <param name="repeat">True is repeated, false if not</param>
        void RepeatMedia(bool repeat);

        #endregion Media Repeat

        #region Media Controls

        /// <summary>
        ///     Skip to the next media in
        ///     the playlist.
        /// </summary>
        void NextMedia();

        /// <summary>
        ///     Play the previous media in
        ///     the playlist.
        /// </summary>
        void PreviousMedia();

        /// <summary>
        ///     Pause the current media.
        /// </summary>
        void PauseMedia();

        /// <summary>
        ///     Plays the current media.
        /// </summary>
        void PlayMedia();

        /// <summary>
        ///     Move to the passed in media (only if the
        ///     media is in the queue).
        /// </summary>
        /// <param name="track">The track to move to.</param>
        void MoveToMedia(Media media);

        #endregion Media Controls

        #region Media Information

        /// <summary>
        ///     Get the duration of the media.
        /// </summary>
        /// <returns>The media duration</returns>
        TimeSpan GetMediaDuration();

        /// <summary>
        ///     Get the current playback state of the media.
        /// </summary>
        /// <returns>The playback state of the media</returns>
        PlaybackState GetPlaybackState();

        /// <summary>
        ///     Get the current playing media (if any).
        ///     Will be null if nothing is playing
        /// </summary>
        /// <returns>Returns the current playing media</returns>
        Media GetCurrentMedia();

        #endregion Media Information

        #region Queue / Source Information

        /// <summary>
        ///     Get the list of tracks currently in the playlist.
        /// </summary>
        /// <returns>A list of tracks in the playlist.</returns>
        List<Track> GetQueue();

        /// <summary>
        ///     Returns the current source token
        ///     (next items in the list).
        /// </summary>
        /// <returns>The source token</returns>
        string GetToken();

        /// <summary>
        ///     Returns the source which tells SoundByte
        ///     which items to load.
        /// </summary>
        /// <returns>The item loading source.</returns>
        ISource GetSource();

        #endregion Queue / Source Information

        #region Start Media

        /// <summary>
        ///     Start queue at a specific media.
        /// </summary>
        /// <param name="mediaToPlay">The media to play, must exist in the queue.</param>
        /// <param name="startTime">Time to start playing the media</param>
        Task StartMediaAsync(Media? mediaToPlay, TimeSpan? startTime = null);

        /// <summary>
        ///     Start queue at a random media item.
        /// </summary>
        Task StartRandomMediaAsync();

        #endregion Start Media

        #region Initialize

        /// <summary>
        ///     Setup a queue and start playing music
        /// </summary>
        /// <param name="model">The model to get more music</param>
        /// <param name="queue">List of items to play</param>
        /// <param name="token">Token to get more information</param>
        /// <returns>The playback start response</returns>
        Task<PlaybackInitializeResponse> InitializeAsync(ISource model, IEnumerable<Media> queue = null, string token = null);

        #endregion Initialize
    }
}