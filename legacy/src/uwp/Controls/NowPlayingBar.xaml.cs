using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.ViewModels.Playback;
using System;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SoundByte.App.Uwp.Controls
{
    public sealed partial class NowPlayingBar
    {
        public PlaybackViewModel PlaybackViewModel { get; } = new PlaybackViewModel();

        public NowPlayingBar() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlayer().PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
        }

        /// <summary>
        ///     Show the loading UI when a track is loading
        /// </summary>
        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            PlaybackSession_PlaybackStateChangedAsync(sender, args).FireAndForgetSafeAsync();
        }

        private async Task PlaybackSession_PlaybackStateChangedAsync(MediaPlaybackSession sender, object args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                    case MediaPlaybackState.Playing:
                    case MediaPlaybackState.Paused:
                        LoadingBar.Visibility = Visibility.Collapsed;
                        ProgressBar.Visibility = Visibility.Visible;
                        break;

                    case MediaPlaybackState.Opening:
                    case MediaPlaybackState.Buffering:
                        LoadingBar.Visibility = Visibility.Visible;
                        ProgressBar.Visibility = Visibility.Collapsed;
                        break;
                }
            });
        }

        private void ToggleExpand(object sender, RoutedEventArgs e) => ToggleExpandAsync().FireAndForgetSafeAsync();

        private async Task ToggleExpandAsync()
        {
            if (!(App.Shell?.GetName("NowPlaying") is DropShadowPanel panel))
                return;

            // If the main content is hidden, we want to expand
            if (MainContent.Opacity == 0.0f)
            {
                var myDoubleAnimation = new DoubleAnimation
                {
                    To = 1500,
                    From = 450,
                    Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                    EasingFunction = new CubicEase(),
                    EnableDependentAnimation = true
                };

                Storyboard.SetTarget(myDoubleAnimation, panel);
                Storyboard.SetTargetProperty(myDoubleAnimation, "MaxWidth");

                var storyboard = new Storyboard();
                storyboard.Children.Add(myDoubleAnimation);

                await Task.WhenAll(new Task[]
                {
                    MainContent.Fade(1.0f, 250, 0, EasingType.Quadratic).StartAsync(),
                    ExpandToggle.Rotate(0, 9f, 9f, 250, 0, EasingType.Quadratic).StartAsync(),
                    storyboard.BeginAsync()
                });
            }
            else
            {
                var myDoubleAnimation = new DoubleAnimation
                {
                    To = 450,
                    From = 1500,
                    Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                    EasingFunction = new CubicEase(),
                    EnableDependentAnimation = true
                };

                Storyboard.SetTarget(myDoubleAnimation, panel);
                Storyboard.SetTargetProperty(myDoubleAnimation, "MaxWidth");

                var storyboard = new Storyboard();
                storyboard.Children.Add(myDoubleAnimation);

                await Task.WhenAll(new Task[]
                {
                    MainContent.Fade(0.0f, 250, 0, EasingType.Bounce).StartAsync(),
                    ExpandToggle.Rotate(178, 9f, 9f, 250, 0, EasingType.Quadratic).StartAsync(),
                    storyboard.BeginAsync()
                });
            }
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

        private void CastTrack(object sender, RoutedEventArgs e) => CastTrackAsync().FireAndForgetSafeAsync();

        private async Task CastTrackAsync() => await NavigationService.Current.CallDialogAsync<CastTrackDialog>();

        private void ContinuePlaying(object sender, RoutedEventArgs e) => ContinuePlayingAsync().FireAndForgetSafeAsync();

        private async Task ContinuePlayingAsync() => await NavigationService.Current.CallDialogAsync<ContinueOnDeviceDialog>();

        private void GoogleCastTrack(object sender, RoutedEventArgs e) => GoogleCastTrackAsync().FireAndForgetSafeAsync();

        private async Task GoogleCastTrackAsync() => await NavigationService.Current.CallDialogAsync<GoogleCastTrackDialog>();

        private void AirPlayCastTrack(object sender, RoutedEventArgs e) => AirPlayCastTrackAsync().FireAndForgetSafeAsync();

        private async Task AirPlayCastTrackAsync() => await NavigationService.Current.CallDialogAsync<AirPlayCastTrackDialog>();
    }
}