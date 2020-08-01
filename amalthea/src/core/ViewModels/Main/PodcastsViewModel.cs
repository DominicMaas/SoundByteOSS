using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Services.Definitions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Main
{
    public class PodcastsViewModel : MvxViewModel
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
        ///     Allows the user to import a podcast url into SoundByte
        /// </summary>
        public MvxCommand ImportPodcastCommand { get; }

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> PageContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Create a new instance of "PodcastsViewModel" and bind the refresh command
        /// </summary>
        public PodcastsViewModel(IContentService contentService)
        {
            // Bind the refresh command
            RefreshCommand = new MvxCommand(() =>
            {
                // Set the content
                PageContent.Clear();
                contentService.GetContentByLocation(ContentArea.Podcasts).ToList().ForEach(group => PageContent.Add(group));

                try
                {
                    IsLoading = true;
                    foreach (var item in PageContent)
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