using SoundByte.Core.Sources;
using SoundByte.Core.Sources.Generic;
using SoundByte.Core.Sources.Podcast;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube;
using SoundByte.Core.Sources.YouTube.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundByte.Core.Managers
{
    /// <summary>
    ///     This class is used for registering sources that are used within
    ///     the app. This is used to help keep track of sources and the data
    ///     they contain in more complex scenarios (e.g Windows Timeline)
    /// </summary>
    public class SourceManager
    {
        private Dictionary<string, Type> _sources = new Dictionary<string, Type>();

        public void RegisterDefaultSources()
        {
            RegisterSource<PodcastItemSource>();
            RegisterSource<PodcastSearchSource>();
            RegisterSource<DummyTrackSource>();
            RegisterSource<SoundCloudSearchPlaylistSource>();
            RegisterSource<SoundCloudSearchTrackSource>();
            RegisterSource<SoundCloudSearchUserSource>();
            RegisterSource<SoundCloudExploreSource>();
            RegisterSource<SoundCloudUserLikeSource>();
            RegisterSource<SoundCloudStreamSource>();

            RegisterSource<ExploreYouTubeTrendingSource>();
            RegisterSource<YouTubeSearchPlaylistSource>();
            RegisterSource<YouTubeSearchTrackSource>();
            RegisterSource<YouTubeSearchUserSource>();
            RegisterSource<YouTubeLikeSource>();
            RegisterSource<YouTubePlaylistSource>();
            RegisterSource<GenericPlaylistItemSource>();
            RegisterSource<SoundCloudTrackStreamSource>();
            RegisterSource<SoundCloudLikedPlaylistSource>();

            // User Sources
            RegisterSource<UserFollowerSource>();
            RegisterSource<UserFollowingSource>();
            RegisterSource<UserPlaylistSource>();
            RegisterSource<UserTrackSource>();
            RegisterSource<UserLikeSource>();

            // Related tracks. Used for searches and other places
            RegisterSource<RelatedTrackSource>();
        }

        public void RegisterSource<T>() where T : ISource
        {
            _sources.Add(typeof(T).Name, typeof(T));
        }

        public ISource GetTrackSource(string name, Dictionary<string, object> data = null)
        {
            var item = _sources.FirstOrDefault(x => x.Key == name);

            if (item.Value == null)
                throw new Exception($"Source not found. ({name})\nPlease report this issue.");

            var instance = Activator.CreateInstance(item.Value);

            if (data != null)
            {
                ((ISource)instance).ApplyParameters(data);
            }

            return (ISource)instance;
        }
    }
}