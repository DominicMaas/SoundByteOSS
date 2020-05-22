using SoundByte.Core.Items.User;
using System;
using Windows.System;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls.ListControls
{
    public sealed partial class UserListItem
    {
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register(nameof(User), typeof(BaseUser), typeof(UserListItem), null);

        /// <summary>
        ///     Gets or sets the user
        /// </summary>
        public BaseUser User
        {
            get => (BaseUser)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public UserListItem()
        {
            InitializeComponent();
        }

        private async void OpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(User.Link));
        }
    }
}