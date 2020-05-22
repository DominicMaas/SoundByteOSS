using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views
{
    /// <summary>
    ///     This page lets the user search for tracks/playlists/people
    ///     within SoundCloud.
    /// </summary>
    public sealed partial class SearchView
    {
        public SearchViewModel ViewModel => (SearchViewModel)DataContext;

        public SearchView() => InitializeComponent();

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        /// <param name="e">Search arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.SearchQuery = e.Parameter != null ? e.Parameter as string : string.Empty;
            PageTitle.Text = $"Results for \"{ViewModel.SearchQuery}\"";

            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("Search View");
        }
    }
}