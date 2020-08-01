using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls.Media.List
{
    public sealed partial class PlaylistListViewItem : UserControl
    {
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register(nameof(Playlist), typeof(Core.Models.Media.Playlist), typeof(PlaylistListViewItem), null);

        public Core.Models.Media.Playlist Playlist
        {
            get => (Core.Models.Media.Playlist)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public PlaylistListViewItem() => InitializeComponent();
    }
}
