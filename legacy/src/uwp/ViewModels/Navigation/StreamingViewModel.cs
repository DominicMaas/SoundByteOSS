using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.Core.Models.Content;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.ViewModels.Navigation
{
    public class MyMusicViewModel : BaseViewModel
    {
        public ObservableCollection<ContentGroup> LikesContent { get; } = new ObservableCollection<ContentGroup>();

        public ObservableCollection<ContentGroup> PlaylistsContent { get; } = new ObservableCollection<ContentGroup>();

        public MyMusicViewModel(IContentService contentService)
        {
            contentService.GetContentGroups(ContentArea.StreamingLikes).ToList().ForEach(group => LikesContent.Add(group));
            contentService.GetContentGroups(ContentArea.StreamingPlaylists).ToList().ForEach(group => PlaylistsContent.Add(group));
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var contentGroup in LikesContent)
            {
                contentGroup.Refresh();
            }

            foreach (var contentGroup in PlaylistsContent)
            {
                contentGroup.Refresh();
            }
        }
    }
}