using Foundation;
using MobileCoreServices;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Main;
using System;
using System.IO;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Main
{
    [MvxTabPresentation(WrapInNavigationController = true, TabIconName = "me", TabName = "Me")]
    [MvxFromStoryboard("Main")]
    public partial class MeView : MvxTableViewController<MeViewModel>
    {
        public MeView(IntPtr handle) : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Me";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            var set = this.CreateBindingSet<MeView, MeViewModel>();
            set.Bind(SoundByteAccountLabel).To(vm => vm.SoundByteAccountStatus).OneWay();
            set.Bind(VersionLabel).To(vm => vm.Version).OneWay();
            set.Bind(BuildDateLabel).To(vm => vm.BuildDate).OneWay();
            set.Apply();
        }

        public override async void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            // Deselect the row
            tableView.DeselectRow(indexPath, true);

            // Perform the specified action
            switch (indexPath.Section)
            {
                // Accounts
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.ToggleSoundByteAccountCommand.ExecuteAsync();
                                break;

                            case 1:
                                await ViewModel.NavigateMusicProviderAccountsCommand.ExecuteAsync();
                                break;
                        }

                        break;
                    }

                // Music Providers
                case 1:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.NavigateInstalledMusicProviderCommand.ExecuteAsync();
                                break;

                            case 1:
                                await ViewModel.NavigateBrowseMusicProviderCommand.ExecuteAsync();
                                break;

                            case 2:
                                {
                                    // Open folder picker
                                    var picker = new UIDocumentPickerViewController(new string[] { UTType.Folder }, UIDocumentPickerMode.Open);
                                    picker.DidPickDocumentAtUrls += (s, e) =>
                                    {
                                        var securityEnabled = e.Urls[0].StartAccessingSecurityScopedResource();

                                        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        var temp = Path.Combine(documents, "..", "tmp", "music-providers");
                                        var tempLocation = Path.Combine(temp, Guid.NewGuid().ToString());

                                        NSFileManager.DefaultManager.CreateDirectory(temp, true, new NSFileAttributes());
                                        var result = NSFileManager.DefaultManager.Copy(e.Urls[0], new NSUrl(tempLocation, true), out var error);

                                        // Load music provider
                                        ViewModel.LoadMusicProviderCommand.Execute(tempLocation);

                                        e.Urls[0].StopAccessingSecurityScopedResource();
                                    };

                                    // Show picker
                                    picker.ModalPresentationStyle = UIModalPresentationStyle.Popover;
                                    PresentViewController(picker, true, null);

                                    break;
                                }
                        }

                        break;
                    }

                // Premium
                case 2:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.NavigateManagePremiumCommand.ExecuteAsync();
                                break;
                        }

                        break;
                    }

                // Settings
                case 3:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.NavigateSettingsCommand.ExecuteAsync();
                                break;
                        }

                        break;
                    }

                // Misc buttons
                case 5:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.SendFeedbackCommand.ExecuteAsync();
                                break;

                            case 1:
                                await ViewModel.OpenReviewDialogCommand.ExecuteAsync();
                                break;

                            case 2:
                                await ViewModel.OpenChangelogCommand.ExecuteAsync();
                                break;
                        }

                        break;
                    }

                // Links
                case 6:
                    {
                        switch (indexPath.Row)
                        {
                            case 0:
                                await ViewModel.OpenTwitterCommand.ExecuteAsync();
                                break;

                            case 1:
                                await ViewModel.OpenWebsiteCommand.ExecuteAsync();
                                break;

                            case 2:
                                await ViewModel.OpenPrivacyPolicyCommand.ExecuteAsync();
                                break;
                        }

                        break;
                    }
            }
        }
    }
}