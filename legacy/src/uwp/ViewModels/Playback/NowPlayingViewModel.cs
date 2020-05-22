using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage.AccessCache;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using YoutubeExplode.Models.MediaStreams;

namespace SoundByte.App.Uwp.ViewModels.Playback
{
    public class NowPlayingViewModel : BaseViewModel
    {
        #region Public Variables

        /// <summary>
        ///     The Playback view model
        /// </summary>
        public PlaybackViewModel PlaybackViewModel
        {
            get => _playbackViewModel;
            private set
            {
                if (_playbackViewModel == value)
                    return;

                _playbackViewModel = value;
                UpdateProperty();
            }
        }

        private PlaybackViewModel _playbackViewModel;

        #endregion Public Variables

        #region View Model Events

        /// <summary>
        ///     Setup the view model
        /// </summary>
        public async Task SetupModelAsync()
        {
            PlaybackViewModel = new PlaybackViewModel();

            // Bind the method once we know a playback list exists
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange += Instance_OnCurrentTrackChanged;

            var currentTrack = SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack();
            if (currentTrack == null)
                return;

            await SetupMediaElementAsync(currentTrack);
        }

        /// <summary>
        ///     Setup the media element to play any YouTube videos.
        /// </summary>
        /// <param name="track">The (youtube video) to play</param>
        private async Task SetupMediaElementAsync(BaseTrack track)
        {
            // Get the video overlay
            if (App.CurrentFrame.FindName("VideoOverlay") is MediaElement overlay)
            {
                // Only run this if video playback is enabled
                if (!SettingsService.Instance.DisableVideoPlayback)
                {
                    // If a YouTube track
                    if (track.ServiceType == ServiceTypes.YouTube)
                    {
                        // Get the media streams for this YouTube video
                        var mediaStreams = await SimpleIoc.Default.GetInstance<IPlaybackService>().GetYoutubeClient().GetVideoMediaStreamInfosAsync(track.TrackId);

                        // If this track is live, we want to use the HLS live stream URL
                        if (track.IsLive)
                        {
                            // Get the source and set it.
                            var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(mediaStreams.HlsLiveStreamUrl));
                            if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                                overlay.SetMediaStreamSource(source.MediaSource);
                        }
                        else
                        {
                            // Start at 1080p
                            var videoStreamUrl = mediaStreams.Video.FirstOrDefault(x => x.VideoQuality == VideoQuality.High1080)?.Url;

                            // If this stream does not exist, choose the next highest
                            if (string.IsNullOrEmpty(videoStreamUrl))
                                videoStreamUrl = mediaStreams.Video.OrderBy(s => s.VideoQuality).Last()?.Url;

                            // Set the sources
                            overlay.Source = new Uri(videoStreamUrl);
                        }

                        // Play the video
                        overlay.Play();
                    }
                    // If a local track and a video (.mp4)
                    else if (track.ServiceType == ServiceTypes.Local && (string)track.CustomProperties["extension"] == ".mp4")
                    {
                        var fileToken = track.CustomProperties["file_token"].ToString();
                        var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken);

                        // Set the source
                        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            overlay.SetSource(stream, "video/mp4");
                        }

                        // Play the video
                        overlay.Play();
                    }
                    else
                    {
                        overlay.Opacity = 0;
                        overlay.Pause();
                        overlay.Source = null;
                    }
                }
                else
                {
                    overlay.Opacity = 0;
                    overlay.Pause();
                    overlay.Source = null;
                }
            }
        }

        /// <summary>
        ///     Dispose of events and resources.
        /// </summary>
        public override void Dispose()
        {
            // Dispose of playback view model
            PlaybackViewModel?.Dispose();
            PlaybackViewModel = null;

            if (App.CurrentFrame?.FindName("VideoOverlay") is MediaElement overlay)
            {
                overlay.Opacity = 0;
                overlay.Pause();
                overlay.Source = null;
            }

            // Unbind the events
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange -= Instance_OnCurrentTrackChanged;
        }

        #endregion View Model Events

        #region Event Handlers

        /// <summary>
        ///     Called when the current playing item changes
        /// </summary>
        private async void Instance_OnCurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                await SetupMediaElementAsync(newTrack);
            });
        }

        #endregion Event Handlers

        #region Method Bindings

        /// <summary>
        ///     Navigate to the selected track in the playlist
        /// </summary>
        public void GotoRelatedTrack(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().MoveToTrack(((BaseSoundByteItem)e.ClickedItem).Track);
        }

        /// <summary>
        ///     Toggle if the app is fullscreen or not.
        /// </summary>
        public void ToggleFullScreen()
        {
            if (!DeviceHelper.IsDeviceFullScreen)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            else
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        #endregion Method Bindings
    }
}