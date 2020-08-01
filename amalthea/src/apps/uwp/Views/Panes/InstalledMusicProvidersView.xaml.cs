using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.ViewModels.Panes;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Views.Panes
{
    [SoundByteModal("Installed Music Providers")]
    [MvxViewFor(typeof(InstalledMusicProvidersViewModel))]
    public sealed partial class InstalledMusicProvidersView : MvxWindowsPage
    {
        public InstalledMusicProvidersViewModel Vm => (InstalledMusicProvidersViewModel)ViewModel;

        public InstalledMusicProvidersView() => InitializeComponent();

        private async void UninstallMusicProvider(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mp = (MusicProvider)((Button)sender).DataContext;
            await Vm.UninstallCommand.ExecuteAsync(mp);
        }

        private async void ViewMusicProvider(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mp = (MusicProvider)((Button)sender).DataContext;
            await Launcher.LaunchUriAsync(new Uri($"https://soundbytemedia.com/store/details?id={mp.Identifier}"));
        }
    }
}
