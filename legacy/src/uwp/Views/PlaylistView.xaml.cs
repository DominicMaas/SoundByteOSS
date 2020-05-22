using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Items.Playlist;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views
{
    /// <summary>
    ///     Displays a playlist
    /// </summary>
    public sealed partial class PlaylistView
    {
        // Page View Model
        public PlaylistViewModel ViewModel = new PlaylistViewModel();

        public PlaylistView()
        {
            // Setup the XAML
            InitializeComponent();
            // Set the data context
            DataContext = ViewModel;

            Unloaded += (s, e) =>
            {
                ViewModel.Dispose();
            };
        }

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //  var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("PlaylistImage");
            //   imageAnimation?.TryStart(PlaylistImageHolder, new[] { TitlePanel });

            // Make sure the view is ready for the user
            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("Playlist View");
            ViewModel.SetupView((BasePlaylist)e.Parameter);
        }
    }
}