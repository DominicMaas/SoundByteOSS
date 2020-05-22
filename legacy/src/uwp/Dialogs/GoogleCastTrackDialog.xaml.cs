using GalaSoft.MvvmLight.Ioc;
using GoogleCast;
using GoogleCast.Channels;
using GoogleCast.Models.Media;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class GoogleCastTrackDialog : ContentDialog
    {
        private DeviceLocator _deviceLocator;

        public ObservableCollection<IReceiver> CastingDevices { get; } = new ObservableCollection<IReceiver>();

        public GoogleCastTrackDialog()
        {
            InitializeComponent();

            _deviceLocator = new DeviceLocator();
        }

        private void CastToDevice(object sender, ItemClickEventArgs e) => CastToDeviceAsync(e).FireAndForgetSafeAsync();

        private async Task CastToDeviceAsync(ItemClickEventArgs e)
        {
            var sender = new Sender();

            // Connect to the Chromecast
            await sender.ConnectAsync(e.ClickedItem as IReceiver);

            // Launch the default media receiver application
            var mediaChannel = sender.GetChannel<IMediaChannel>();
            await sender.LaunchAsync(mediaChannel);

            // Load and play Big Buck Bunny video
            var mediaStatus = await mediaChannel.LoadAsync(
                new MediaInformation() { ContentId = SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack().AudioStreamUrl });
        }

        private void Load(object sender, RoutedEventArgs e) => LoadAsync().FireAndForgetSafeAsync();

        private async Task LoadAsync()
        {
            try
            {
                LoadingRing.Visibility = Visibility.Visible;

                var receivers = await _deviceLocator.FindReceiversAsync();

                CastingDevices.Clear();

                foreach (var receiver in receivers)
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
    }
}