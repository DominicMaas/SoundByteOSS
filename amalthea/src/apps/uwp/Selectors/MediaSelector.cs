using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Selectors
{
    /// <summary>
    ///     Used to select the correct template to display on the screen
    /// </summary>
    public class MediaSelector : DataTemplateSelector
    {
        public DataTemplate TrackTemplate { get; set; }

        public DataTemplate UserTemplate { get; set; }

        public DataTemplate PlaylistTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var vm = (Core.Models.Media.Media)item;

            if (vm is Core.Models.Media.Track)
                return TrackTemplate;

            if (vm is Core.Models.Media.User)
                return UserTemplate;

            if (vm is Core.Models.Media.Playlist)
                return PlaylistTemplate;

            throw new System.Exception("Unknown item type. No template selected");
        }
    }
}