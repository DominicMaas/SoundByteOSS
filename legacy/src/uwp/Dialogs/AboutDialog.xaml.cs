using Newtonsoft.Json;
using SoundByte.Core.Items;
using SoundByte.App.Uwp.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void AboutDialog_Loaded(object sender, RoutedEventArgs e) => OnLoadedAsync().FireAndForgetSafeAsync();

        private async Task OnLoadedAsync()
        {
            // Set the app version
            AppVersion.Text = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

            AppBuildTime.Text = "(unknown build time)";

            var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\build_info.json");
            var buildData =
                await Task.Run(() => JsonConvert.DeserializeObject<BuildInformation>(File.ReadAllText(dataFile.Path)));

            AppBuildTime.Text = buildData.BuildTime;
        }
    }
}