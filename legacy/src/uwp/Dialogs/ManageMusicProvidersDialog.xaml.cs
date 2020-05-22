using SoundByte.Core;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Views;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class ManageMusicProvidersDialog
    {
        public ManageMusicProvidersDialog()
        {
            InitializeComponent();
            RefreshUi();
        }

        private void RefreshUi()
        {
            if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
            {
                SoundCloudSignInButton.Visibility = Visibility.Collapsed;
                SoundCloudSignOutButton.Visibility = Visibility.Visible;
                SoundCloudViewProfileButton.Visibility = Visibility.Visible;
            }
            else
            {
                SoundCloudSignInButton.Visibility = Visibility.Visible;
                SoundCloudSignOutButton.Visibility = Visibility.Collapsed;
                SoundCloudViewProfileButton.Visibility = Visibility.Collapsed;
            }

            if (SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
            {
                YouTubeSignInButton.Visibility = Visibility.Collapsed;
                YouTubeSignOutButton.Visibility = Visibility.Visible;
                YouTubeViewProfileButton.Visibility = Visibility.Visible;
            }
            else
            {
                YouTubeSignInButton.Visibility = Visibility.Visible;
                YouTubeSignOutButton.Visibility = Visibility.Collapsed;
                YouTubeViewProfileButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SoundCloudSignIn(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(AccountView), new AccountView.AccountViewParams
            {
                Service = ServiceTypes.SoundCloud,
            });
            Hide();
        }

        private void SoundCloudViewProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteService.Current.GetConnectedUser(ServiceTypes.SoundCloud));
            Hide();
        }

        private void SoundCloudSignOut(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceTypes.SoundCloud, string.Empty);
            RefreshUi();
        }

        private void YouTubeSignIn(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(AccountView), new AccountView.AccountViewParams
            {
                Service = ServiceTypes.YouTube,
            });
            Hide();
        }

        private void YouTubeViewProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteService.Current.GetConnectedUser(ServiceTypes.YouTube));
            Hide();
        }

        private void YouTubeSignOut(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceTypes.YouTube, string.Empty);
            RefreshUi();
        }
    }
}