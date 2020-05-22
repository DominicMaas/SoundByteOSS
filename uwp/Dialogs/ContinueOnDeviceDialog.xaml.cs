using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class ContinueOnDeviceDialog
    {
        private RemoteSystemWatcher _remoteSystemWatcher;

        /// <summary>
        ///     Remote Systems
        /// </summary>
        public ObservableCollection<RemoteSystem> RemoteSystems { get; } = new ObservableCollection<RemoteSystem>();

        public ContinueOnDeviceDialog()
        {
            InitializeComponent();

            SetupRemoteWatcher();
        }

        private async void RemoteSystemWatcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var index = RemoteSystems.IndexOf(RemoteSystems.FirstOrDefault(rs => rs.Id == args.RemoteSystem.Id));

                if (index > -1)
                {
                    RemoteSystems[index] = args.RemoteSystem;
                }
            });
        }

        private async void RemoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                RemoteSystems.Remove(RemoteSystems.FirstOrDefault(rs => rs.Id == args.RemoteSystemId));
            });
        }

        private async void RemoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                RemoteSystems.Add(args.RemoteSystem);
            });
        }

        public async void RemoteSystemItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                LoadingPane.Visibility = Windows.UI.Xaml.Visibility.Visible;
                var system = (RemoteSystem)e.ClickedItem;

                if (SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack() != null)
                {
                    var track = new BaseSoundByteItem(SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack());
                    var playlist = SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlaybackList().Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack()));
                    var token = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistToken();
                    var source = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistSource();

                    var status = await RemoteLauncher.LaunchUriAsync(new RemoteSystemConnectionRequest(system),
                        new Uri(ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(source, track, playlist, token, SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackDuration()), true)));

                    if (status == RemoteLaunchUriStatus.Success)
                    {
                        Hide();
                        LoadingPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        Hide();
                        LoadingPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        await NavigationService.Current.CallMessageDialogAsync("Failed with status: " + status, "Remote System Error");
                    }
                }
                else
                {
                    Hide();
                    LoadingPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    await NavigationService.Current.CallMessageDialogAsync("A track must be playing", "Remote System Error");
                }
            }
            finally
            {
                LoadingPane.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void SetupRemoteWatcher()
        {
            var accessStatus = await RemoteSystem.RequestAccessAsync();
            if (accessStatus == RemoteSystemAccessStatus.Allowed)
            {
                // Create and start the remote system
                _remoteSystemWatcher = RemoteSystem.CreateWatcher();

                // Bind methods
                _remoteSystemWatcher.RemoteSystemAdded += RemoteSystemWatcher_RemoteSystemAdded;
                _remoteSystemWatcher.RemoteSystemRemoved += RemoteSystemWatcher_RemoteSystemRemoved;
                _remoteSystemWatcher.RemoteSystemUpdated += RemoteSystemWatcher_RemoteSystemUpdated;

                _remoteSystemWatcher.Start();
            }
        }
    }
}