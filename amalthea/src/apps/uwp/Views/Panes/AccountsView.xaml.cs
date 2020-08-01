using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.ViewModels.Panes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Views.Panes
{
    [SoundByteModal("Accounts")]
    [MvxViewFor(typeof(AccountsViewModel))]
    public sealed partial class AccountsView : MvxWindowsPage
    {
        public AccountsViewModel Vm => (AccountsViewModel)ViewModel;

        public AccountsView() => InitializeComponent();

        private async void ToggleAccount(object sender, RoutedEventArgs e)
        {
            var mp = (MusicProviderAccount)((Button)sender).DataContext;
            await Vm.InvokeCommand.ExecuteAsync(mp);
        }
    }
}
