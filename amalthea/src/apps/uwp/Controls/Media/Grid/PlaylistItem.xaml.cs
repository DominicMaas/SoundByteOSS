using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.App.UWP.Controls.Media.Grid
{
    public sealed partial class PlaylistItem : UserControl
    {
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register(nameof(Playlist), typeof(Core.Models.Media.Playlist), typeof(PlaylistItem), null);

        public Core.Models.Media.Playlist Playlist
        {
            get => (Core.Models.Media.Playlist)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public PlaylistItem() => InitializeComponent();

        private void ShowHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.ShowHoverAnimation(ShadowPanel, HoverArea);

        private void HideHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.HideHoverAnimation(ShadowPanel, HoverArea);
    }
}