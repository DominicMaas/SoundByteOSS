using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Controls.Media.List
{
    public sealed partial class UserListViewItem : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(nameof(User), typeof(Core.Models.Media.User), typeof(UserListViewItem), null);

        public Core.Models.Media.User User
        {
            get => (Core.Models.Media.User)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public UserListViewItem() => InitializeComponent();
    }
}
