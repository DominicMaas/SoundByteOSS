namespace SoundByte.Core.Models.Playback
{
    public static class PlaybackServiceEventHandlers
    {
        public delegate void MediaChangedEventHandler(Media.Media media);

        public delegate void StateChangedEventHandler(PlaybackState mediaPlaybackState);
    }
}