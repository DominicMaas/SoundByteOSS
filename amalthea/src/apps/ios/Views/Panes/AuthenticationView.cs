using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Panes;
using System;
using UIKit;
using WebKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Panes
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.Popover, WrapInNavigationController = true)]
    public class AuthenticationView : MvxViewController<AuthenticationViewModel>
    {
        private WKWebView _webView;
        private IDisposable _urlObs;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Connect Account";
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            var config = new WKWebViewConfiguration();
            config.ApplicationNameForUserAgent = "Version/8.0.2 Safari/600.2.5";

            // Create the webview
            _webView = new WKWebView(View.Frame, config);

            _urlObs = _webView.AddObserver("URL", NSKeyValueObservingOptions.New, async (o) =>
              {
                  var newUrl = _webView.Url.ToString();
                  await ViewModel.HandleRequestCommand.ExecuteAsync(newUrl);
              });

            View.AddSubview(_webView);

            // Navigate to the request url
            var url = new NSUrl(ViewModel.AuthenticationDetails.ConnectUrl);
            var request = new NSUrlRequest(url);
            _webView.LoadRequest(request);

            // Cancel button
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, delegate { ViewModel.CloseCommand.Execute(); });
        }

        public async override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            _urlObs.Dispose();
            await ViewModel.CloseCommand.ExecuteAsync();
        }
    }
}