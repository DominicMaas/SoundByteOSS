using SoundByte.Core.Extension.Attributes;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Models.Extension;

namespace SoundByte.Core.Extension
{
    public interface IExtensionPlayback
    {
        [ApiMetadata(ApiVersion.V1, "Pause the current playing track if there are any.")]
        void PauseTrack();

        [ApiMetadata(ApiVersion.V1, "Plays the track (if any tracks are paused).")]
        void PlayTrack();

        [ApiMetadata(ApiVersion.V1, "Play the next track.")]
        void NextTrack();

        [ApiMetadata(ApiVersion.V1, "Play the previous track.")]
        void PreviousTrack();

        [ApiMetadata(ApiVersion.V1, "Get the current playing track.")]
        BaseTrack GetPlayingTrack();
    }
}