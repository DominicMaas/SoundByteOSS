using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.App.UWP.Controls.Media.Grid
{
    public sealed partial class UserItem : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(nameof(User), typeof(Core.Models.Media.User), typeof(UserItem), null);

        public Core.Models.Media.User User
        {
            get => (Core.Models.Media.User)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public UserItem() => InitializeComponent();

        private void ShowHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.ShowHoverAnimation(ShadowPanel, HoverArea);

        private void HideHoverAnimation(object sender, PointerRoutedEventArgs e) => MediaItem.HideHoverAnimation(ShadowPanel, HoverArea);
    }
}