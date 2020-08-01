using CoreGraphics;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels;
using SoundByte.Core.ViewModels.Main;
using System;
using System.Collections.Generic;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.IoC;
using SoundByte.App.iOS.Controls;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Services.Implementations;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views
{
    [MvxRootPresentation]
    public class RootView : MvxTabBarViewController<RootViewModel>
    {
        private readonly bool _constructed;

        private nfloat _tabBarHeight;

        private NowPlayingBar _nowPlayingBar;

        public RootView()
        {
            _constructed = true;

            // Need this additional call to ViewDidLoad because
            // UIkit creates the view before the C# hierarchy has been constructed
            ViewDidLoad();
        }

        public override void ViewDidLoad()
        {
            if (!_constructed)
                return;

            base.ViewDidLoad();

            if (ViewModel == null)
                return;

            // Values that are needed
            var controllers = new List<UIViewController>();

            // Grab the settings service
            var settingsService = Mvx.IoCProvider.Resolve<ISettingsService>();

            // Get the app mode (by default the app mode is both)
            var appMode = settingsService.GetPreference(SettingsService.AppMode, AppMode.Both);

            // The home and discover tabs always exist
            controllers.Add(CreateTabFor(0, "Home", "home", typeof(HomeViewModel)));
            controllers.Add(CreateTabFor(1, "Discover", "discover", typeof(DiscoverViewModel)));

            switch (appMode)
            {
                case AppMode.Both:
                    controllers.Add(CreateTabFor(2, "Podcasts", "podcasts", typeof(PodcastsViewModel)));
                    controllers.Add(CreateTabFor(3, "My Music", "my-music", typeof(MyMusicViewModel)));
                    break;

                case AppMode.Media:
                    controllers.Add(CreateTabFor(2, "My Music", "my-music", typeof(MyMusicViewModel)));
                    break;

                case AppMode.Podcast:
                    controllers.Add(CreateTabFor(2, "Podcasts", "podcasts", typeof(PodcastsViewModel)));
                    break;
            }

            // The me tab is always required
            var correctIndex = controllers.Count;
            controllers.Add(CreateTabFor(correctIndex, "Me", "me", typeof(MeViewModel)));

            ViewControllers = controllers.ToArray();
            CustomizableViewControllers = new UIViewController[] { };

            SelectedViewController = ViewControllers[0];

            // Shadow
            TabBar.Layer.ShadowColor = UIColor.Black.CGColor;
            TabBar.Layer.ShadowOpacity = 0.8f;
            TabBar.Layer.ShadowOffset = new CGSize(0, -2);
            TabBar.Layer.ShadowRadius = 6.0f;

            // Border
            TabBar.Layer.BorderWidth = 0;
            TabBar.Layer.BorderColor = UIColor.Clear.CGColor;

            TabBar.ClipsToBounds = true;
            TabBar.BackgroundColor = ColorHelper.Background4DP.ToPlatformColor();

            _tabBarHeight = TabBar.Frame.Size.Height + 8.0f;

            // Now playing bar
            ViewModel.GetPlaybackService().OnMediaChange += RootView_OnMediaChange;
        }

        private void RootView_OnMediaChange(Core.Models.Media.Media media)
        {
            ShowNowPlayingBar();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            var newFrame = TabBar.Frame;

            var newSize = newFrame.Size;
            newSize.Height = _tabBarHeight;
            newFrame.Size = newSize;

            var newLocation = newFrame.Location;
            newLocation.Y = View.Frame.Size.Height - _tabBarHeight;
            newFrame.Location = newLocation;

            TabBar.Frame = newFrame;
        }

        private void ShowNowPlayingBar()
        {
            if (_nowPlayingBar != null)
                return;

            var viewModelLoader = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>();
            var viewModel = (PlaybackViewModel)viewModelLoader.LoadViewModel(new MvxViewModelRequest<PlaybackViewModel>(), null);

            _nowPlayingBar = new NowPlayingBar()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DataContext = viewModel
            };

            View.AddSubview(_nowPlayingBar);
            View.BringSubviewToFront(_nowPlayingBar);

            _nowPlayingBar.HeightAnchor.ConstraintEqualTo(NowPlayingBar.Height).Active = true;
            _nowPlayingBar.BottomAnchor.ConstraintEqualTo(TabBar.TopAnchor, -12).Active = true;
            _nowPlayingBar.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 12).Active = true;
            _nowPlayingBar.RightAnchor.ConstraintEqualTo(View.RightAnchor, -12).Active = true;
        }

        private UIViewController CreateTabFor(int index, string title, string imageName, Type viewModelType)
        {
            var controller = new UINavigationController();
            var request = new MvxViewModelRequest(viewModelType, null, null);
            //var viewModel = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>().LoadViewModel(request, null);
            var screen = this.CreateViewControllerFor(request) as UIViewController;

            screen.Title = title;
            screen.TabBarItem = new UITabBarItem(title, UIImage.FromBundle(imageName), index);
            controller.PushViewController(screen, true);
            return controller;
        }
    }
}