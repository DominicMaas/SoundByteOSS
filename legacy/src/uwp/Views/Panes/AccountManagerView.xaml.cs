using SoundByte.Core;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.Views.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Panes
{
    /// <summary>
    ///     This page allows the user to manage connected accounts.
    /// </summary>
    public sealed partial class AccountManagerView
    {
        public AccountManagerView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshUi();
        }

        private void RefreshUi()
        {
            // All the linked accounts we could have
            if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundByte))
            {
                SoundByteSignInButton.Visibility = Visibility.Collapsed;
                SoundByteSignOutButton.Visibility = Visibility.Visible;
            }
            else
            {
                SoundByteSignInButton.Visibility = Visibility.Visible;
                SoundByteSignOutButton.Visibility = Visibility.Collapsed;
            }
        }

        #region Remote

        private async void SetupThisDevice(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallMessageDialogAsync("This feature has not been implemented yet,", "Not Supported");
        }

        private async void SetupAnotherDevice(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallMessageDialogAsync("This feature has not been implemented yet,", "Not Supported");
        }

        #endregion Remote

        #region SoundByte

        private void SoundByteSignIn(object sender, RoutedEventArgs e)
        {
            (App.Shell as DesktopShell)?.CloseSidePane();
            App.NavigateTo(typeof(AccountView), new AccountView.AccountViewParams
            {
                Service = ServiceTypes.SoundByte,
            });
        }

        private void SoundByteSignOut(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceTypes.SoundByte, string.Empty);
            RefreshUi();
        }

        #endregion SoundByte

        #region Account Providers

        private async void ManageMusicProviders(object sender, RoutedEventArgs e)
        {
            (App.Shell as DesktopShell)?.CloseSidePane();
            await NavigationService.Current.CallDialogAsync<ManageMusicProvidersDialog>();
        }

        private async void ManageExtensionAccounts(object sender, RoutedEventArgs e)
        {
            (App.Shell as DesktopShell)?.CloseSidePane();
            await NavigationService.Current.CallDialogAsync<ManageExtensionAccountsDialog>();
        }

        #endregion Account Providers
    }
}