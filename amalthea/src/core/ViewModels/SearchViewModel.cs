using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources.Podcasts;

namespace SoundByte.Core.ViewModels
{
    /// <summary>
    ///     View model used for search for new content
    /// </summary>
    public class SearchViewModel : MvxViewModel<string>
    {
        /// <summary>
        ///     Command that will cause content groups to be refreshed
        /// </summary>
        public MvxCommand RefreshCommand { get; }

        /// <summary>
        ///     Is the UI currently loading
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isLoading;

        /// <summary>
        ///     The text that was searched for
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _searchText;

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> TracksContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> PlaylistsContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> UsersContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     A list of podcast search results
        /// </summary>
        public SoundByteCollection<PodcastSearchSource> PodcastsContent { get; } = new SoundByteCollection<PodcastSearchSource>();

        /// <summary>
        ///     Create a new instance of "SearchViewModel" and bind the refresh command
        /// </summary>
        public SearchViewModel(IContentService contentService)
        {
            // Bind the refresh command
            RefreshCommand = new MvxCommand(() =>
            {
                try
                {
                    // Set the content
                    TracksContent.Clear();
                    contentService.GetContentByLocation(ContentArea.SearchTracks).ToList().ForEach(group => TracksContent.Add(group));

                    PlaylistsContent.Clear();
                    contentService.GetContentByLocation(ContentArea.SearchPlaylists).ToList().ForEach(group => PlaylistsContent.Add(group));

                    UsersContent.Clear();
                    contentService.GetContentByLocation(ContentArea.SearchUsers).ToList().ForEach(group => UsersContent.Add(group));

                    IsLoading = true;

                    foreach (var item in TracksContent)
                    {
                        item.Collection.Source.ApplyParameters(new Dictionary<string, string> { { "query", SearchText } });
                        item.Refresh();
                    }

                    foreach (var item in PlaylistsContent)
                    {
                        item.Collection.Source.ApplyParameters(new Dictionary<string, string> { { "query", SearchText } });
                        item.Refresh();
                    }

                    foreach (var item in UsersContent)
                    {
                        item.Collection.Source.ApplyParameters(new Dictionary<string, string> { { "query", SearchText } });
                        item.Refresh();
                    }

                    PodcastsContent.Source.ApplyParameters(new Dictionary<string, string> { { "query", SearchText } });
                    PodcastsContent.RefreshItems();
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        public override void Prepare(string parameter)
        {
            SearchText = parameter;
        }

        public override Task Initialize()
        {
            RefreshCommand.Execute();
            return Task.CompletedTask;
        }
    }
}