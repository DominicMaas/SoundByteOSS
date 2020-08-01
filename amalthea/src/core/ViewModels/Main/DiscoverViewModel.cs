using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Services.Definitions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Main
{
    public class DiscoverViewModel : MvxViewModel
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
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> MediaContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> PodcastsContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Create a new instance of "BrowseViewModel" and bind the refresh command
        /// </summary>
        public DiscoverViewModel(IContentService contentService)
        {
            // Bind the refresh command
            RefreshCommand = new MvxCommand(() =>
            {
                // Set the content
                MediaContent.Clear();
                contentService.GetContentByLocation(ContentArea.DiscoverMedia).ToList().ForEach(group => MediaContent.Add(group));
                PodcastsContent.Clear();
                contentService.GetContentByLocation(ContentArea.DiscoverPodcasts).ToList().ForEach(group => PodcastsContent.Add(group));

                try
                {
                    IsLoading = true;
                    foreach (var item in MediaContent)
                    {
                        item.Refresh();
                    }
                    foreach (var item in PodcastsContent)
                    {
                        item.Refresh();
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        public override Task Initialize()
        {
            RefreshCommand.Execute();
            return Task.CompletedTask;
        }
    }
}