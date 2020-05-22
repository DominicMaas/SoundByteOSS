using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels.Panes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Panes
{
    /// <summary>
    ///     This is the main settings/about page for the app.
    ///     is handled here
    /// </summary>
    public sealed partial class SettingsPaneView
    {
        // View model for the settings page
        public SettingsPaneViewModel ViewModel { get; } = new SettingsPaneViewModel();

        /// <summary>
        ///     Setup the page
        /// </summary>
        public SettingsPaneView()
        {
            // Initialize XAML Components
            InitializeComponent();
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        // The settings object, we bind to this to change values
        public SettingsService SettingsService { get; set; } = SettingsService.Instance;

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("Settings View");

            // TEMP, Load the page
            LoadSettingsPage();
        }

        private void LoadSettingsPage()
        {
            ViewModel.IsComboboxBlockingEnabled = true;
            // Get the saved language
            var appLanguage = SettingsService.Instance.CurrentAppLanguage;
            // Check that the string is not empty
            if (!string.IsNullOrEmpty(appLanguage))
                switch (appLanguage)
                {
                    case "en-US":
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;

                    case "fr":
                        LanguageComboBox.SelectedItem = Language_French_FR;
                        break;

                    case "nl":
                        LanguageComboBox.SelectedItem = Language_Dutch_NL;
                        break;

                    default:
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;
                }
            else
                LanguageComboBox.SelectedItem = Language_English_US;

            switch (SettingsService.Instance.ApplicationThemeType)
            {
                case AppTheme.Default:
                    ThemeComboBox.SelectedItem = DefaultTheme;
                    break;

                case AppTheme.Light:
                    ThemeComboBox.SelectedItem = LightTheme;
                    break;

                case AppTheme.Dark:
                    ThemeComboBox.SelectedItem = DarkTheme;
                    break;

                default:
                    ThemeComboBox.SelectedItem = DefaultTheme;
                    break;
            }

            // Set the start page combo box
            switch (SettingsService.Instance.StartPage)
            {
                case "home":
                    StartPageComboBox.SelectedItem = StartPageHome;
                    break;

                case "browse":
                    StartPageComboBox.SelectedItem = StartPageBrowse;
                    break;

                case "podcasts":
                    StartPageComboBox.SelectedItem = StartPagePodcasts;
                    break;

                case "my-music":
                    StartPageComboBox.SelectedItem = StartPageMyMusic;
                    break;

                default:
                    StartPageComboBox.SelectedItem = StartPageHome;
                    break;
            }

            // Enable combo boxes
            ViewModel.IsComboboxBlockingEnabled = false;
        }

        private void AppThemeComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.IsComboboxBlockingEnabled)
                return;

            switch (((ComboBoxItem)(sender as ComboBox)?.SelectedItem)?.Name.ToLower())
            {
                case "defaulttheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Default;
                    ((Page)Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;

                case "darktheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Dark;
                    ((Page)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;

                case "lighttheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Light;
                    ((Page)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;

                default:
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Default;
                    ((Page)Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;
            }

            TitlebarHelper.UpdateTitlebarStyle();
        }
    }
}