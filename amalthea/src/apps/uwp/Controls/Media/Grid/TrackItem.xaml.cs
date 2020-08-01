using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.App.UWP.Controls.Media.Grid
{
    public sealed partial class TrackItem : UserControl
    {
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register(nameof(Track), typeof(Core.Models.Media.Track), typeof(TrackItem), null);

        public Core.Models.Media.Track Track
        {
            get => (Core.Models.Media.Track)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        public TrackItem() => InitializeComponent();

        private void ShowHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.ShowHoverAnimation(ShadowPanel, HoverArea);

        private void HideHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.HideHoverAnimation(ShadowPanel, HoverArea);
    }
}