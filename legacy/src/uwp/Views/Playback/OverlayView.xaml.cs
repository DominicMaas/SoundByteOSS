using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels.Playback;
using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Playback
{
    public sealed partial class OverlayView
    {
        public PlaybackViewModel PlaybackViewModel { get; private set; }
        private int _mainAppViewId;

        public OverlayView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlaybackViewModel = new PlaybackViewModel(CoreWindow.GetForCurrentThread().Dispatcher);
            _mainAppViewId = (int)e.Parameter;

            // Set the accent color
            TitlebarHelper.UpdateTitlebarStyle();

            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("Compact Overlay View");
        }

        private async void NavigateToMainView()
        {
            var currentViewId = -1;

            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentViewId = ApplicationView.GetForCurrentView().Id;
            });

            await ApplicationViewSwitcher.TryShowAsViewModeAsync(_mainAppViewId, ApplicationViewMode.Default);

            // Switch to this window
            await ApplicationViewSwitcher.SwitchAsync(_mainAppViewId, currentViewId,
                ApplicationViewSwitchingOptions.ConsolidateViews);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PlaybackViewModel?.Dispose();
            PlaybackViewModel = null;
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlaybackViewModel = new PlaybackViewModel(CoreWindow.GetForCurrentThread().Dispatcher);
        }

        private void Page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlaybackViewModel?.Dispose();
            PlaybackViewModel = null;
        }
    }
}