using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.YouTube.Search;
using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.ServicesV2;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using SoundByte.Core.Models.Content;

namespace SoundByte.App.Uwp.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public ObservableCollection<ContentGroup> TracksContent { get; } = new ObservableCollection<ContentGroup>();

        public ObservableCollection<ContentGroup> PlaylistsContent { get; } = new ObservableCollection<ContentGroup>();

        public ObservableCollection<ContentGroup> UsersContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     The current pivot item that the user is viewing
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value != _searchQuery)
                {
                    _searchQuery = value;
                    UpdateProperty();

                    // Tracks
                    ((SoundCloudSearchTrackSource)TracksContent.First(x => x.Collection.Source.GetType() == typeof(SoundCloudSearchTrackSource)).Collection.Source).SearchQuery = value;
                    ((YouTubeSearchTrackSource)TracksContent.First(x => x.Collection.Source.GetType() == typeof(YouTubeSearchTrackSource)).Collection.Source).SearchQuery = value;

                    // Playlists
                    ((SoundCloudSearchPlaylistSource)PlaylistsContent.First(x => x.Collection.Source.GetType() == typeof(SoundCloudSearchPlaylistSource)).Collection.Source).SearchQuery = value;
                    ((YouTubeSearchPlaylistSource)PlaylistsContent.First(x => x.Collection.Source.GetType() == typeof(YouTubeSearchPlaylistSource)).Collection.Source).SearchQuery = value;

                    // Users
                    ((SoundCloudSearchUserSource)UsersContent.First(x => x.Collection.Source.GetType() == typeof(SoundCloudSearchUserSource)).Collection.Source).SearchQuery = value;
                    ((YouTubeSearchUserSource)UsersContent.First(x => x.Collection.Source.GetType() == typeof(YouTubeSearchUserSource)).Collection.Source).SearchQuery = value;

                    // Refresh
                    Refresh(this, null);
                }
            }
        }

        // The query string
        private string _searchQuery;

        public SearchViewModel(IContentService contentService)
        {
            contentService.GetContentGroups(ContentArea.SearchTracks).ToList().ForEach(group => TracksContent.Add(group));
            contentService.GetContentGroups(ContentArea.SearchPlaylists).ToList().ForEach(group => PlaylistsContent.Add(group));
            contentService.GetContentGroups(ContentArea.SearchUsers).ToList().ForEach(group => UsersContent.Add(group));
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var contentGroup in TracksContent)
            {
                contentGroup.Refresh();
            }

            foreach (var contentGroup in PlaylistsContent)
            {
                contentGroup.Refresh();
            }

            foreach (var contentGroup in UsersContent)
            {
                contentGroup.Refresh();
            }
        }
    }
}