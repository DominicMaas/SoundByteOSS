using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ServicesV2;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class ShareDialog
    {
        public ShareDialog(BaseTrack trackItem)
        {
            // Do this before the xaml is loaded, to make sure
            // the object can be binded to.
            Track = trackItem;

            // Load the XAML page
            InitializeComponent();
        }

        public BaseTrack Track { get; }

        private void ShareWindows(object sender, RoutedEventArgs e)
        {
            // Create a share event
            void ShareEvent(DataTransferManager s, DataRequestedEventArgs a)
            {
                var dataPackage = a.Request.Data;
                dataPackage.Properties.Title = "SoundByte";
                dataPackage.Properties.Description = "Share this track with Windows 10.";
                dataPackage.SetText("Listen to " + Track.Title + " by " + Track.User.Username +
                                    " on @SoundByteUWP " + Track.Link);
            }

            // Remove any old share events
            DataTransferManager.GetForCurrentView().DataRequested -= ShareEvent;
            // Add this new share event
            DataTransferManager.GetForCurrentView().DataRequested += ShareEvent;
            // Show the share dialog
            DataTransferManager.ShowShareUI();
            // Hide the popup
            Hide();
            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Share Menu - Windows Share");
        }

        private void ShareLink(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var data = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            // Set the link to the track on soundcloud
            data.SetText(Track.Link);
            // Set the clipboard content
            Clipboard.SetContent(data);
            // Hide the popup
            Hide();
            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Share Menu - Copy General Link");
        }

        private void ShareSoundByte(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            // Set the link to the track on soundcloud
            dataPackage.SetText($"soundbyte://track?d={ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(null, new BaseSoundByteItem(Track)), false)}");
            // Set the clipboard content
            Clipboard.SetContent(dataPackage);
            // Hide the popup
            Hide();
            // Track Event
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Share Menu - Copy SoundByte Link");
        }
    }
}