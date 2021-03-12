using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Xbox
{
    public sealed partial class XboxSettingsView : Page
    {
        // The settings object, we bind to this to change values
        public SettingsService SettingsService { get; set; } = SettingsService.Instance;

        private bool _isComboboxBlockingEnabled;

        public XboxSettingsView()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Load settings
            LoadSettingsPage();

            // Set the app version
            AppVersion.Text = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

            AppBuildTime.Text = "(unknown build time)";

            var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\build_info.json");
            var buildData = await Task.Run(() => JsonConvert.DeserializeObject<BuildInformation>(File.ReadAllText(dataFile.Path)));

            AppBuildTime.Text = buildData.BuildTime;
        }

        private void LoadSettingsPage()
        {
            _isComboboxBlockingEnabled = true;

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

            // Enable combo boxes
            _isComboboxBlockingEnabled = false;
        }

        private void AppThemeComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isComboboxBlockingEnabled)
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
        }

        private async void ManageMusicProviders(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<ManageMusicProvidersDialog>();
        }
    }
}