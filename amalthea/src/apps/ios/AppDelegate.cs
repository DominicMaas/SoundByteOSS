using Firebase.RemoteConfig;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Platforms.Ios.Core;
using SoundByte.Core;
using SoundByte.Core.Helpers;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate<Setup, Core.App>
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Start AppCenter
            AppCenter.Start(Constants.AppCenterIosKey, typeof(Analytics), typeof(Crashes));

            // Start Firebase
            Firebase.Core.App.Configure();
            Firebase.Crashlytics.Crashlytics.Configure();

            // Default colors
            UITableView.Appearance.BackgroundColor = ColorHelper.Background0.ToPlatformColor();
            UITableViewCell.Appearance.BackgroundColor = ColorHelper.Background1DP.ToPlatformColor();
            UINavigationBar.Appearance.LargeTitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };

            // Tab bar
            UITabBar.Appearance.SelectedImageTintColor = ColorHelper.Accent.ToPlatformColor();
            UITabBar.Appearance.BackgroundColor = ColorHelper.Background4DP.ToPlatformColor();
            UITabBar.Appearance.BarTintColor = UIColor.Clear;
            UITabBar.Appearance.ShadowImage = new UIImage();
            UITabBar.Appearance.BackgroundImage = new UIImage();

            // Color of the selected tab text color:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes
                {
                    TextColor = ColorHelper.Accent.ToPlatformColor()
                },
                UIControlState.Selected);

            // Color of the unselected tab icon & text:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes
                {
                    TextColor = UIColor.FromRGB(146, 146, 146)
                },
                UIControlState.Normal);

            UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0, -4);
            
#pragma warning disable 618
            // Bug: https://github.com/xamarin/GoogleApisForiOSComponents/issues/368
            RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings(true);
#pragma warning restore 618
            
            return result;
        }
    }
}