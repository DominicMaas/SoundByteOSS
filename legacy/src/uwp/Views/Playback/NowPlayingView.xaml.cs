using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Playback
{
    /// <summary>
    ///     This page handles track playback and connection to
    ///     the background audio task.
    /// </summary>
    public sealed partial class NowPlayingView
    {
        // Main page view model
        public NowPlayingViewModel ViewModel { get; } = new NowPlayingViewModel();

        /// <summary>
        ///     Setup page
        /// </summary>
        public NowPlayingView()
        {
            InitializeComponent();
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        /// <summary>
        ///     Setup the view model, passing in the navigation events.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Setup view model
            await ViewModel.SetupModelAsync();
            MoreInfoPivot.SelectedItem = AboutPivotItem;
        }

        /// <summary>
        ///     Clean the view model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Dispose();
        }

        private void VideoOverlay_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            VideoOverlay.Position = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition();
            VideoOverlay.Fade(1, 450).Start();
        }
    }
}