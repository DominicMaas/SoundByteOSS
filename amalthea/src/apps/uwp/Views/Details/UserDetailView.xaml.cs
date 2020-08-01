using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels.Details;

namespace SoundByte.App.UWP.Views.Details
{
    [MvxViewFor(typeof(UserDetailViewModel))]
    public sealed partial class UserDetailView : MvxWindowsPage
    {
        public UserDetailViewModel Vm => (UserDetailViewModel)ViewModel;

        public UserDetailView() => InitializeComponent();
    }
}
