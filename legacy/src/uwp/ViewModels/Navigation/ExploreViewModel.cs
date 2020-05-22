using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.Core.Models.Content;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.ViewModels.Navigation
{
    /// <summary>
    ///     View model for the explore page
    /// </summary>
    public class BrowseViewModel : BaseViewModel
    {
        public ObservableCollection<ContentGroup> PageContent { get; } = new ObservableCollection<ContentGroup>();

        public BrowseViewModel(IContentService contentService)
        {
            contentService.GetContentGroups(ContentArea.ExploreTracks).ToList().ForEach(group => PageContent.Add(group));
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var contentGroup in PageContent)
            {
                contentGroup.Refresh();
            }
        }
    }
}