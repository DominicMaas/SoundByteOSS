using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.Core.Models.Content;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.ViewModels.Navigation
{
    /// <summary>
    ///     View model for the home page
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<ContentGroup> PageContent { get; } = new ObservableCollection<ContentGroup>();

        public HomeViewModel(IContentService contentService)
        {
            contentService.GetContentGroups(ContentArea.ForYou).ToList().ForEach(group => PageContent.Add(group));
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            // Refresh the app content
            foreach (var contentGroup in PageContent)
            {
                contentGroup.Refresh();
            }
        }
    }
}