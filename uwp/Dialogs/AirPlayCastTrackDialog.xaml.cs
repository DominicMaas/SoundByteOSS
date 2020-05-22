using SoundByte.App.Uwp.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using Zeroconf;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class AirPlayCastTrackDialog : ContentDialog
    {
        public ObservableCollection<IZeroconfHost> CastingDevices { get; } = new ObservableCollection<IZeroconfHost>();

        public AirPlayCastTrackDialog()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e) => LoadAsync().FireAndForgetSafeAsync();

        private async Task LoadAsync()
        {
            try
            {
                LoadingRing.Visibility = Visibility.Visible;

                var domains = await ZeroconfResolver.BrowseDomainsAsync();

                var results = await ZeroconfResolver.ResolveAsync(domains.Where(x => x.Key.Contains("airplay")).Select(g => g.Key));

                CastingDevices.Clear();

                foreach (var receiver in results)
                {
                    CastingDevices.Add(receiver);
                }

                NoDevicesTextBlock.Visibility = CastingDevices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        private void CastToDevice(object sender, ItemClickEventArgs e) => CastToDeviceAsync(e).FireAndForgetSafeAsync();

        private async Task CastToDeviceAsync(ItemClickEventArgs e)
        {
            var device = e.ClickedItem as IZeroconfHost;

            await new MessageDialog($"IP: {device.IPAddress}\nDisplay Name:{device.DisplayName}\nPort:{device.Services.First().Value.Port}\nTTL:{device.Services.First().Value.Ttl}", "Device Info").ShowAsync();
        }
    }
}