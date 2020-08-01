using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Implementations
{
    public class TrackService : ITrackService
    {
        public Task<string> GetAudioStreamUrl(Track track)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVideoStreamUrl(Track track)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasLikedAsync(Track track)
        {
            return Task.FromResult(false);
        }

        public Task LikeAsync(Track track)
        {
            return Task.CompletedTask;
        }

        public Task<Track> ResolveTrackAsync(MusicProvider musicProvider, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> ResolveTracksAsync(MusicProvider musicProvider, string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task UnlikeAsync(Track track)
        {
            return Task.CompletedTask;
        }
    }
}
