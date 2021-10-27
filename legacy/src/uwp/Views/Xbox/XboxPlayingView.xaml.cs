using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels.Playback;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using YoutubeExplode.Videos.Streams;

namespace SoundByte.App.Uwp.Views.Xbox
{
    public sealed partial class XboxPlayingView
    {
        public PlaybackViewModel ViewModel => (PlaybackViewModel)DataContext;

        public XboxPlayingView() => InitializeComponent();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Bind on source change
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange += Instance_OnCurrentTrackChanged;

            // Get the current track (if playing)
            var currentTrack = SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack();

            // Show a pane that says to start playing music
            if (currentTrack == null)
            {
                PlaybackGrid.Visibility = Visibility.Collapsed;
                StartPlaybackGrid.Visibility = Visibility.Visible;

                return;
            }
            else
            {
                PlaybackGrid.Visibility = Visibility.Visible;
                StartPlaybackGrid.Visibility = Visibility.Collapsed;
            }

            // Setup the media element to start playing music if applicable
            await SetupMediaElementAsync(currentTrack);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unbind Events
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange -= Instance_OnCurrentTrackChanged;

            // Stop playing the video
            VideoOverlay.Opacity = 0;
            VideoOverlay.Pause();
            VideoOverlay.Source = null;

            BackgroundBlur.Visibility = Visibility.Visible;
            Overlay.Opacity = 0;

            // Dispose VM
            ViewModel.Dispose();

            if (SimpleIoc.Default.IsRegistered<PlaybackViewModel>())
                SimpleIoc.Default.Unregister<PlaybackViewModel>();
        }

        private async void Instance_OnCurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                // Show a pane that says to start playing music
                if (newTrack == null)
                {
                    PlaybackGrid.Visibility = Visibility.Collapsed;
                    StartPlaybackGrid.Visibility = Visibility.Visible;

                    return;
                }
                else
                {
                    PlaybackGrid.Visibility = Visibility.Visible;
                    StartPlaybackGrid.Visibility = Visibility.Collapsed;
                }

                await SetupMediaElementAsync(newTrack);
            });
        }

        /// <summary>
        ///     Setup the media element to play any YouTube videos.
        /// </summary>
        /// <param name="track">The (YouTube video) to play</param>
        private async Task SetupMediaElementAsync(BaseTrack track)
        {
            try
            {
                // Don't run if video playback is disabled
                if (!SettingsService.Instance.DisableVideoPlayback)
                {
                    // If the track is YouTube
                    if (track.ServiceType == ServiceTypes.YouTube)
                    {
                        // If this track is live, we want to use the HLS live stream URL
                        if (track.IsLive)
                        {
                            var liveStreamUrl = await SimpleIoc.Default.GetInstance<IPlaybackService>().GetYoutubeClient()
                                .Videos.Streams.GetHttpLiveStreamUrlAsync(track.TrackId);

                            // Get the source and set it.
                            var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(liveStreamUrl));
                            if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                                VideoOverlay.SetMediaStreamSource(source.MediaSource);
                        }
                        else
                        {
                            // Get the media streams for this YouTube video
                            var manifest = await SimpleIoc.Default.GetInstance<IPlaybackService>().GetYoutubeClient().Videos.Streams.GetManifestAsync(track.TrackId);

                            // Start at 1080p
                            var videoStreamUrl = manifest.GetVideoStreams().FirstOrDefault(x => x.VideoQuality.IsHighDefinition)?.Url;

                            // If this stream does not exist, choose the next highest
                            if (string.IsNullOrEmpty(videoStreamUrl))
                                videoStreamUrl = manifest.GetVideoStreams().OrderBy(s => s.VideoQuality).Last()?.Url;

                            // Set the sources
                            VideoOverlay.Source = new Uri(videoStreamUrl);
                        }

                        // Play the video
                        VideoOverlay.Play();

                        return;
                    }
                    // If the track is local and video.
                    else if (track.ServiceType == ServiceTypes.Local && (string)track.CustomProperties["extension"] == ".mp4")
                    {
                        var fileToken = track.CustomProperties["file_token"].ToString();
                        var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken);

                        // Set the source
                        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            VideoOverlay.SetSource(stream, "video/mp4");
                        }

                        // Play the video
                        VideoOverlay.Play();
                        return;
                    }
                }
            }
            catch
            {
                // If video cannot be loaded, don't worry about it
                VideoOverlay.Opacity = 0;
                VideoOverlay.Pause();
                VideoOverlay.Source = null;

                BackgroundBlur.Visibility = Visibility.Visible;
                Overlay.Opacity = 0;
            }

            VideoOverlay.Opacity = 0;
            VideoOverlay.Pause();
            VideoOverlay.Source = null;

            BackgroundBlur.Visibility = Visibility.Visible;
            Overlay.Opacity = 0;
        }

        private void VideoOverlay_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            VideoOverlay.Position = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition();
            VideoOverlay.Fade(1, 450).Start();

            BackgroundBlur.Visibility = Visibility.Collapsed;
            Overlay.Opacity = 1;
        }

        private void ChangePlaybackRate(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton)?.Tag)
            {
                case "0.25":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 0.25;
                    break;

                case "0.5":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 0.5;
                    break;

                case "0.75":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 0.75;
                    break;

                case "normal":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 1.0;
                    break;

                case "1.25":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 1.25;
                    break;

                case "1.5":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 1.5;
                    break;

                case "2":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackRate = 2;
                    break;
            }
        }

        private async void ContinuePlaying(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<ContinueOnDeviceDialog>();
        }
    }
}