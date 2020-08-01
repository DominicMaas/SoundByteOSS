using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Panes;
using System;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Panes
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.Popover, WrapInNavigationController = true)]
    [MvxFromStoryboard("Main")]
    public partial class SettingsView : MvxTableViewController<SettingsViewModel>
    {
        public SettingsView(IntPtr handle) : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Settings";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Cancel button
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, delegate { ViewModel.CloseCommand.Execute(); });
        }

        public async override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            await ViewModel.CloseCommand.ExecuteAsync();
        }
    }
}