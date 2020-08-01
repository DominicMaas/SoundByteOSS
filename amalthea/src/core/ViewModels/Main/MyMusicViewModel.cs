using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Services.Definitions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Main
{
    public class MyMusicViewModel : MvxViewModel
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
        public ObservableCollection<ContentGroup> LikesContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Content to be displayed on this page
        /// </summary>
        public ObservableCollection<ContentGroup> PlaylistsContent { get; } = new ObservableCollection<ContentGroup>();

        /// <summary>
        ///     Create a new instance of "MyMusicViewModel" and bind the refresh command
        /// </summary>
        public MyMusicViewModel(IContentService contentService)
        {
            // Bind the refresh command
            RefreshCommand = new MvxCommand(() =>
            {
                try
                {
                    // Set the content
                    LikesContent.Clear();
                    contentService.GetContentByLocation(ContentArea.MyMusicLikes).ToList().ForEach(group => LikesContent.Add(group));

                    PlaylistsContent.Clear();
                    contentService.GetContentByLocation(ContentArea.MyMusicPlaylists).ToList().ForEach(group => PlaylistsContent.Add(group));

                    IsLoading = true;

                    foreach (var item in LikesContent)
                    {
                        item.Refresh();
                    }

                    foreach (var item in PlaylistsContent)
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