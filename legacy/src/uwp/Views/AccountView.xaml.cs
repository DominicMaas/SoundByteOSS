using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views
{
    public sealed partial class AccountView
    {
        public class AccountViewParams
        {
            public int Service { get; set; }
        }

        private int _loginService;
        private string _stateVerification;

        public AccountView()
        {
            InitializeComponent();

            // Loading event handlers
            LoginWebView.NavigationStarting += (sender, a) => { LoadingSection.Visibility = Visibility.Visible; };
            LoginWebView.NavigationCompleted += (sender, a) => { LoadingSection.Visibility = Visibility.Collapsed; };

            // Handle new window requests, if a new window is requested, just navigate on the
            // current page.
            LoginWebView.NewWindowRequested += (view, eventArgs) =>
            {
                eventArgs.Handled = true;
                LoginWebView.Navigate(eventArgs.Uri);
            };
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = (AccountViewParams)e.Parameter;

            // Set the login service type
            _loginService = args.Service;
            _stateVerification = new Random().Next(0, 100000000).ToString("D8");

            // Create the URI
            string connectUri;

            // Hide new login system for now
            AccountV3Section.Visibility = Visibility.Collapsed;

            switch (_loginService)
            {
                //soundbyte://authorization/lastfm

                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    connectUri = $"https://soundcloud.com/connect?scope=non-expiring&client_id={AppKeys.SoundCloudClientId}&response_type=code&display=popup&redirect_uri={AppKeys.AppLegacyCallback}&state={_stateVerification}";
                    break;

                case ServiceTypes.YouTube:
                    connectUri = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={AppKeys.YouTubeLoginClientId}&redirect_uri={AppKeys.AppLegacyCallback}&response_type=code&state={_stateVerification}&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fyoutube";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            //// Only these services are supported at the moment
            //if (_loginService == ServiceType.SoundByte
            //    || _loginService == ServiceType.Spotify)
            //{
            //    AccountV3Section.Visibility = Visibility.Visible;
            //    await Launcher.LaunchUriAsync(new Uri(connectUri));
            //}
            //else
            //{
            // Clear cache
            await WebView.ClearTemporaryWebDataAsync();

            // Show the web view and navigate to the connect URI
            LoginWebView.Visibility = Visibility.Visible;
            LoginWebView.Navigate(new Uri(connectUri));
            // }
        }

        private async void LoginWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs eventArgs)
        {
            if (eventArgs.Uri.Host == "localhost")
            {
                // Cancel the navigation, (as localhost does not exist).
                eventArgs.Cancel = true;

                // Parse the URL for work
                // ReSharper disable once CollectionNeverUpdated.Local
                var parser = new QueryParameterCollection(eventArgs.Uri);

                // First we just check that the state equals (to make sure the url was not hijacked)
                var state = parser.FirstOrDefault(x => x.Key == "state").Value;

                // The state does not match
                if (string.IsNullOrEmpty(state) || state.TrimEnd('#') != _stateVerification)
                {
                    // Display the error to the user
                    await NavigationService.Current.CallMessageDialogAsync(
                        "State Verification Failed. This could be caused by another process intercepting the SoundByte login procedure. Signin has been canceled to protect your privacy.",
                        "Sign in Error");
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("State Verification Failed");

                    // Close
                    Frame.GoBack();
                    return;
                }

                // We have an error
                if (parser.FirstOrDefault(x => x.Key == "error").Value != null)
                {
                    var type = parser.FirstOrDefault(x => x.Key == "error").Value;
                    var reason = parser.FirstOrDefault(x => x.Key == "error_description").Value;

                    // The user denied the request
                    if (type == "access_denied")
                    {
                        Frame.GoBack();

                        return;
                    }

                    // Display the error to the user
                    await NavigationService.Current.CallMessageDialogAsync(reason, "Sign in Error");
                    Frame.GoBack();
                    return;
                }

                if (parser.FirstOrDefault(x => x.Key == "code").Value == null)
                {
                    await NavigationService.Current.CallMessageDialogAsync("No Code", "Sign in Error");
                    Frame.GoBack();
                    return;
                }

                // Get the code
                var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                try
                {
                    // Get the login token
                    var loginToken = await AuthorizationHelpers.GetAuthTokenAsync(_loginService, code);

                    // Connect the service
                    SoundByteService.Current.ConnectService(_loginService, loginToken);

                    // Close
                    Frame.GoBack();
                }
                catch (SoundByteException ex)
                {
                    // Display the error to the user
                    await NavigationService.Current.CallMessageDialogAsync(ex.ErrorDescription, ex.ErrorTitle);

                    // Close
                    Frame.GoBack();
                }
            }
        }
    }
}