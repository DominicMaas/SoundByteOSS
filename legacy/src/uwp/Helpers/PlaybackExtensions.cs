using SoundByte.Core.Items.Track;
using Windows.Media.Core;

namespace SoundByte.App.Uwp.Helpers
{
    public static class PlaybackExtensions
    {
        public static MediaSource AsMediaSource(this BaseTrack track, MediaSource baseMediaSource)
        {
            baseMediaSource.CustomProperties["SOUNDBYTE_ITEM"] = track;
            return baseMediaSource;
        }

        public static BaseTrack AsBaseTrack(this MediaSource source)
        {
            return source.CustomProperties["SOUNDBYTE_ITEM"] as BaseTrack;
        }
    }
}