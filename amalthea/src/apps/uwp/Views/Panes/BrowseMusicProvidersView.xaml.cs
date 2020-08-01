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
    [SoundByteModal("Find Music Providers")]
    [MvxViewFor(typeof(BrowseMusicProvidersViewModel))]
    public sealed partial class BrowseMusicProvidersView : MvxWindowsPage
    {
        public BrowseMusicProvidersViewModel Vm => (BrowseMusicProvidersViewModel)ViewModel;

        public BrowseMusicProvidersView() => InitializeComponent();

        private void SearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Vm.SearchUpdateCommand.Execute();
        }

        private async void InstallMusicProvider(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mp = (WebMusicProvider)((Button)sender).DataContext;
            await Vm.InstallCommand.ExecuteAsync(mp);
        }

        private async void ViewMusicProvider(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mp = (WebMusicProvider)((Button)sender).DataContext;
            await Launcher.LaunchUriAsync(new Uri($"https://soundbytemedia.com/store/details?id={mp.Id}"));
        }
    }
}
