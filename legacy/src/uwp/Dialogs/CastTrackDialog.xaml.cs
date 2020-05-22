using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.Media.Casting;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Dialogs
{
    /// <summary>
    ///     Dialog allowing the user to cast there music to other devices.
    /// </summary>
    public sealed partial class CastTrackDialog
    {
        private DeviceWatcher _deviceWatcher;
        private CastingConnection _castingConnection;

        /// <summary>
        ///     List of devices to cast to
        /// </summary>
        public ObservableCollection<CastingDevice> CastingDevices { get; } = new ObservableCollection<CastingDevice>();

        #region Constructor / Deconstructor

        public CastTrackDialog()
        {
            // Create the UI
            InitializeComponent();

            // Create a device watcher for audio and video devices
            _deviceWatcher = DeviceInformation.CreateWatcher(CastingDevice.GetDeviceSelector(CastingPlaybackTypes.Audio | CastingPlaybackTypes.Video));

            // Bind the events
            _deviceWatcher.Added += OnDeviceAdd;
            _deviceWatcher.Removed += OnDeviceRemove;
            _deviceWatcher.EnumerationCompleted += OnDeviceWatcherComplete;
            _deviceWatcher.Stopped += OnDeviceWatcherStop;

            // Start looking for devices
            _deviceWatcher.Start();

            LoadingRing.Visibility = Visibility.Visible;
        }

        ~CastTrackDialog()
        {
            // If null, return
            if (_deviceWatcher == null)
                return;

            // Stop any watching
            _deviceWatcher.Stop();

            // Unbind the events
            _deviceWatcher.Added -= OnDeviceAdd;
            _deviceWatcher.Removed -= OnDeviceRemove;
            _deviceWatcher.EnumerationCompleted -= OnDeviceWatcherComplete;
            _deviceWatcher.Stopped -= OnDeviceWatcherStop;

            _deviceWatcher = null;
        }

        #endregion Constructor / Deconstructor

        #region Device Watcher Methods

        private async void OnDeviceRemove(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var currentDevice in CastingDevices)
                {
                    if (currentDevice.Id == args.Id)
                    {
                        CastingDevices.Remove(currentDevice);
                    }
                }

                NoDevicesTextBlock.Visibility = CastingDevices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private async void OnDeviceAdd(DeviceWatcher sender, DeviceInformation args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Add each discovered device to the list
                var addedDevice = await CastingDevice.FromIdAsync(args.Id);
                CastingDevices.Add(addedDevice);

                NoDevicesTextBlock.Visibility = CastingDevices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void OnDeviceWatcherStop(DeviceWatcher sender, object args)
        { }

        private async void OnDeviceWatcherComplete(DeviceWatcher sender, object args)
        {
            _deviceWatcher.Stop();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NoDevicesTextBlock.Visibility = CastingDevices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                LoadingRing.Visibility = Visibility.Collapsed;
            });
        }

        #endregion Device Watcher Methods

        private async void CastToDevice(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Ensure we stop looking for devices before casting
            if (_deviceWatcher.Status != DeviceWatcherStatus.Stopped)
                _deviceWatcher.Stop();

            // Create the connection
            _castingConnection = (e.ClickedItem as CastingDevice).CreateCastingConnection();

            // Bind events
            _castingConnection.ErrorOccurred += OnCastingError;
            _castingConnection.StateChanged += OnCastingStateChange;

            await _castingConnection.RequestStartCastingAsync(SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().GetAsCastingSource());
        }

        private async void OnCastingStateChange(CastingConnection sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (sender.State)
                {
                    case CastingConnectionState.Disconnected:
                        LoadingPane.Visibility = Visibility.Collapsed;
                        break;

                    case CastingConnectionState.Connected:
                    case CastingConnectionState.Rendering:
                        LoadingBar.Visibility = Visibility.Collapsed;
                        LoadingPane.Visibility = Visibility.Visible;
                        LoadingTitle.Text = "Casting...";
                        LoadingText.Text = "Casting music to your device.";
                        LoadingCancelButton.Visibility = Visibility.Visible;
                        LoadingCancelButton.Content = "Disconnect";
                        break;

                    case CastingConnectionState.Disconnecting:
                        LoadingBar.Visibility = Visibility.Visible;
                        LoadingPane.Visibility = Visibility.Visible;
                        LoadingTitle.Text = "Disconnecting...";
                        LoadingText.Text = "Trying to disconnect from the device.";
                        LoadingCancelButton.Visibility = Visibility.Collapsed;
                        break;

                    case CastingConnectionState.Connecting:
                        LoadingBar.Visibility = Visibility.Visible;
                        LoadingPane.Visibility = Visibility.Visible;
                        LoadingTitle.Text = "Connecting...";
                        LoadingText.Text = "Trying to connect to your selected device.";
                        LoadingCancelButton.Visibility = Visibility.Visible;
                        LoadingCancelButton.Content = "Cancel";
                        break;
                }
            });
        }

        private async void OnCastingError(CastingConnection sender, CastingConnectionErrorOccurredEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LoadingBar.Visibility = Visibility.Collapsed;
                LoadingPane.Visibility = Visibility.Visible;
                LoadingTitle.Text = "Error";
                LoadingText.Text = args.Message;
                LoadingCancelButton.Visibility = Visibility.Visible;
                LoadingCancelButton.Content = "Close";
            });
        }

        private async void Cancel(object sender, RoutedEventArgs e)
        {
            LoadingPane.Visibility = Visibility.Collapsed;
            await _castingConnection?.DisconnectAsync();
        }
    }
}