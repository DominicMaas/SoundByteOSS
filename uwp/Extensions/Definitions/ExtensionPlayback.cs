using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Extension;
using SoundByte.Core.Items.Track;
using SoundByte.App.Uwp.ServicesV2;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    public class ExtensionPlayback : IExtensionPlayback
    {
        public void PauseTrack()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().PauseTrack();
        }

        public void PlayTrack()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().PlayTrack();
        }

        public void NextTrack()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().NextTrack();
        }

        public void PreviousTrack()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().PreviousTrack();
        }

        public BaseTrack GetPlayingTrack()
        {
            return SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack();
        }
    }
}