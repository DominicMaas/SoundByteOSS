namespace SoundByte.Core.Models.Playback
{
    /// <summary>
    ///     The current state of media playback
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        ///     No current state.
        /// </summary>
        None,

        /// <summary>
        ///     A media item is opening.
        /// </summary>
        Opening,

        /// <summary>
        ///     A media item is buffering.
        /// </summary>
        Buffering,

        /// <summary>
        ///     A media item is playing.
        /// </summary>
        Playing,

        /// <summary>
        ///     Playback of a media item is paused.
        /// </summary>
        Paused
    }
}