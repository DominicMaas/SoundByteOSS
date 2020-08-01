using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls.Media.List
{
    public sealed partial class TrackListViewItem : UserControl
    {
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register(nameof(Track), typeof(Core.Models.Media.Track), typeof(TrackListViewItem), null);

        public Core.Models.Media.Track Track
        {
            get => (Core.Models.Media.Track)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        public TrackListViewItem() => InitializeComponent();
    }
}
