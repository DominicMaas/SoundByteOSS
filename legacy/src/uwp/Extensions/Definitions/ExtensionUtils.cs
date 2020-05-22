using System;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    public class ExtensionUtils
    {
        public TimeSpan TimeFromMilliseconds(double ms)
        {
            return TimeSpan.FromMilliseconds(ms);
        }

        public BaseSoundByteItem FromUser(BaseUser user)
        {
            return new BaseSoundByteItem(user);
        }

        public BaseSoundByteItem FromPlaylist(BasePlaylist playlist)
        {
            return new BaseSoundByteItem(playlist);
        }

        public BaseSoundByteItem FromTrack(BaseTrack track)
        {
            return new BaseSoundByteItem(track);
        }

        public BaseSoundByteItem FromPodcast(BasePodcast podcast)
        {
            return new BaseSoundByteItem(podcast);
        }
    }
}