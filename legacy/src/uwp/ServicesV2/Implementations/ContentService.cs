using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Generic;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube;
using SoundByte.Core.Sources.YouTube.Search;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.Models.Buttons;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.ViewModels.Generic;
using SoundByte.App.Uwp.Views;
using SoundByte.App.Uwp.Views.Generic;
using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Models.Content;
using SoundByte.App.Uwp.Sources;
using System;

namespace SoundByte.App.Uwp.ServicesV2.Implementations
{
    public class ContentService : IContentService
    {
        private readonly List<ContentItem> _items;

        public IEnumerable<ContentGroup> GetContentGroups(ContentArea location)
        {
            return _items.Where(x => x.Location == location).Select(x => x.Group);
        }

        public IEnumerable<ContentGroup> GetContentByMusicProvider(Guid musicProviderId)
        {
            return _items.Where(x => x.MusicProviderId == musicProviderId).Select(x => x.Group);
        }

        public void RemoveContentByMusicProvider(Guid musicProviderId)
        {
            _items.RemoveAll(x => x.MusicProviderId == musicProviderId);
        }

        /// <summary>
        ///     This is done here for now until we move to a
        ///     fully extension based system.
        /// </summary>
        public ContentService()
        {
            _items = new List<ContentItem>();

            BuildForYouPage();
            BuildExplore();
            BuildStreamingLikes();
            BuildStreamingPlaylists();
            BuildSearchTracks();
            BuildSearchPlaylists();
            BuildSearchUsers();
        }

        private void BuildForYouPage()
        {
            // The users SoundCloud stream
            AddContent(ContentArea.ForYou, new ContentGroup(new SoundCloudStreamSource(), "SoundCloud Stream", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(MixedListView), new GenericListViewModel.Holder(parent.Collection, "SoundCloud Stream"))));
        }

        private void BuildExplore()
        {
            // YouTube Trending
            AddContent(ContentArea.ExploreTracks, new ContentGroup(new ExploreYouTubeTrendingSource(), "Trending YouTube Music Videos", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "YouTube Trending"))));
        }

        private void BuildStreamingLikes()
        {
            // YouTube
            AddContent(ContentArea.StreamingLikes, new ContentGroup(new YouTubeLikeSource(), "YouTube", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Likes"))));

            // SoundCloud
            AddContent(ContentArea.StreamingLikes, new ContentGroup(new SoundCloudUserLikeSource { UserId = SoundByteService.Current.GetConnectedUser(ServiceTypes.SoundCloud)?.UserId }, "SoundCloud", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Likes"))));
        }

        private void BuildStreamingPlaylists()
        {
            // SoundCloud
            AddContent(ContentArea.StreamingPlaylists, new ContentGroup(new SoundCloudLikedPlaylistSource(), "SoundCloud", new List<ContentButton>
            {
                new CreatePlaylistContentButton(ServiceTypes.SoundCloud)
            }, parent => App.NavigateTo(typeof(PlaylistListView), new GenericListViewModel.Holder(parent.Collection, "Playlists"))));

            // YouTube
            AddContent(ContentArea.StreamingPlaylists, new ContentGroup(new YouTubePlaylistSource(), "YouTube", new List<ContentButton>
            {
                new CreatePlaylistContentButton(ServiceTypes.YouTube)
            }, parent => App.NavigateTo(typeof(PlaylistListView), new GenericListViewModel.Holder(parent.Collection, "Playlists"))));
        }

        private void BuildSearchTracks()
        {
            // SoundCloud
            AddContent(ContentArea.SearchTracks, new ContentGroup(new SoundCloudSearchTrackSource(), "SoundCloud", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Search Results")), OnSearchTrackClickCommand));

            // YouTube
            AddContent(ContentArea.SearchTracks, new ContentGroup(new YouTubeSearchTrackSource(), "YouTube", new List<ContentButton>
            {
                new PlayContentButton(),
                new ShufflePlayContentButton()
            }, parent => App.NavigateTo(typeof(TrackListView), new GenericListViewModel.Holder(parent.Collection, "Search Results")), OnSearchTrackClickCommand));
        }

        private async void OnSearchTrackClickCommand(BaseSoundByteItem item)
        {
            // Create the related track collection
            var relatedTrackCollection = new SoundByteCollection<RelatedTrackSource>();
            relatedTrackCollection.Source.Service = item.Track.ServiceType;
            relatedTrackCollection.Source.TrackId = item.Track.TrackId;

            // Add this track as an initial track
            relatedTrackCollection.Add(item);

            // Play the tracks
            await BaseViewModel.PlayAllTracksAsync(relatedTrackCollection, item.Track);
        }

        private void BuildSearchPlaylists()
        {
            // SoundCloud
            AddContent(ContentArea.SearchPlaylists, new ContentGroup(new SoundCloudSearchPlaylistSource(), "SoundCloud", new List<ContentButton>
            {
            }, parent => App.NavigateTo(typeof(PlaylistListView), new GenericListViewModel.Holder(parent.Collection, "Search Results"))));

            // YouTube
            AddContent(ContentArea.SearchPlaylists, new ContentGroup(new YouTubeSearchPlaylistSource(), "YouTube", new List<ContentButton>
            {
            }, parent => App.NavigateTo(typeof(PlaylistListView), new GenericListViewModel.Holder(parent.Collection, "Search Results"))));
        }

        private void BuildSearchUsers()
        {
            // SoundCloud
            AddContent(ContentArea.SearchUsers, new ContentGroup(new SoundCloudSearchUserSource(), "SoundCloud", new List<ContentButton>
            {
            }, parent => App.NavigateTo(typeof(UserListView), new GenericListViewModel.Holder(parent.Collection, "Search Results"))));

            // YouTube
            AddContent(ContentArea.SearchUsers, new ContentGroup(new YouTubeSearchUserSource(), "YouTube", new List<ContentButton>
            {
            }, parent => App.NavigateTo(typeof(UserListView), new GenericListViewModel.Holder(parent.Collection, "Search Results"))));
        }

        public void AddContent(ContentArea location, ContentGroup group)
        {
            _items.Add(new ContentItem(Guid.NewGuid(), location, group));
        }

        public void AddContent(Guid musicProviderId, ContentArea location, ContentGroup group)
        {
            _items.Add(new ContentItem(musicProviderId, location, group));
        }

        private class ContentItem
        {
            public Guid MusicProviderId { get; }

            public ContentArea Location { get; }

            public ContentGroup Group { get; }

            public ContentItem(Guid musicProviderId, ContentArea location, ContentGroup group)
            {
                MusicProviderId = musicProviderId;
                Location = location;
                Group = group;
            }
        }
    }
}