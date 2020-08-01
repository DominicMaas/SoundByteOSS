using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Details;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Details
{
    public class UserDetailView : MvxViewController<UserDetailViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();
        }
    }
}