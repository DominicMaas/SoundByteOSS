using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmCross;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.UWP.Controls
{
    public sealed partial class NowPlayingBar : UserControl
    {
        public PlaybackViewModel ViewModel { get; }

        public NowPlayingBar()
        {
            // Load the viewModel
            var viewModelLoader = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>();
            ViewModel = (PlaybackViewModel)viewModelLoader.LoadViewModel(new MvxViewModelRequest<PlaybackViewModel>(), null);
            InitializeComponent();
        }

        /// <summary>
        ///     Wraps the radio buttons to call the playback rate command
        /// </summary>
        private void ChangePlaybackRate(object sender, RoutedEventArgs e)
        {
            if (double.TryParse((sender as RadioButton)?.Tag?.ToString(), out var rate))
            {
                ViewModel.ChangePlaybackRateCommand.Execute(rate);
            }
        }

        private async void ToggleExpand(object sender, RoutedEventArgs e)
        {
            var panel = Window.Current.Content.FindControl<DropShadowPanel>("NowPlaying");
            if (panel == null)
                return;

            // If the main content is hidden, we want to expand
            if (MainContent.Opacity == 0.0f)
            {
                var myDoubleAnimation = new DoubleAnimation
                {
                    To = 1500,
                    From = 430,
                    Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                    EasingFunction = new CubicEase(),
                    EnableDependentAnimation = true
                };

                Storyboard.SetTarget(myDoubleAnimation, panel);
                Storyboard.SetTargetProperty(myDoubleAnimation, "MaxWidth");

                var storyboard = new Storyboard();
                storyboard.Children.Add(myDoubleAnimation);

                await Task.WhenAll(new[]
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
                    To = 430,
                    From = 1500,
                    Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                    EasingFunction = new CubicEase(),
                    EnableDependentAnimation = true
                };

                Storyboard.SetTarget(myDoubleAnimation, panel);
                Storyboard.SetTargetProperty(myDoubleAnimation, "MaxWidth");

                var storyboard = new Storyboard();
                storyboard.Children.Add(myDoubleAnimation);

                await Task.WhenAll(new[]
                {
                    MainContent.Fade(0.0f, 250, 0, EasingType.Bounce).StartAsync(),
                    ExpandToggle.Rotate(178, 9f, 9f, 250, 0, EasingType.Quadratic).StartAsync(),
                    storyboard.BeginAsync()
                });
            }
        }

        private void ProgressBarPointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ViewModel.MoveMediaPosition.Execute();
        }
    }
}
