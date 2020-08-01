using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels.Panes;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.UWP.Views.Panes
{
    [MvxViewFor(typeof(AuthenticationViewModel))]
    public sealed partial class AuthenticationView : MvxWindowsPage
    {
        public AuthenticationViewModel Vm => (AuthenticationViewModel)ViewModel;

        public AuthenticationView() => InitializeComponent();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Clear the cache
            await WebView.ClearTemporaryWebDataAsync();

            // Navigate to the connect url
            LoginWebView.Navigate(new Uri(Vm.AuthenticationDetails.ConnectUrl));
        }

        private async void LoginWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var url = args.Uri.ToString();
            await Vm.HandleRequestCommand.ExecuteAsync(url);
        }
    }
}
