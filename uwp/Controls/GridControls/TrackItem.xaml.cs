using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.Views;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using UICompositionAnimations.Composition;

namespace SoundByte.App.Uwp.Controls.GridControls
{
    public sealed partial class TrackItem
    {
        /// <summary>
        /// Identifies the Track dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register(nameof(Track), typeof(BaseTrack), typeof(TrackItem), null);

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public BaseTrack Track
        {
            get => (BaseTrack)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        public TrackItem() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) =>
            OnLoadedAsync().FireAndForgetSafeAsync();

        private async Task OnLoadedAsync()
        {
            var gridViewItemElement = this.FindAscendant<GridViewItem>();
            if (gridViewItemElement != null)
            {
                gridViewItemElement.GotFocus += GridViewItemElement_GotFocus;
                gridViewItemElement.LostFocus += GridViewItemElement_LostFocus;
            }

            // If the track has not loaded, don't do anything
            if (Track != null)
            {
                // Update the track like status
                await Track.UpdateLikeStatusAsync();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var gridViewItemElement = this.FindAscendant<GridViewItem>();
            if (gridViewItemElement != null)
            {
                gridViewItemElement.GotFocus -= GridViewItemElement_GotFocus;
                gridViewItemElement.LostFocus -= GridViewItemElement_LostFocus;
            }
        }

        private void GridViewItemElement_LostFocus(object sender, RoutedEventArgs e)
        {
            HideHoverAnimation(sender, null);
        }

        private void GridViewItemElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowHoverAnimation(sender, null);
        }

        private void Share(object sender, RoutedEventArgs e) => ShareAsync().FireAndForgetSafeAsync();

        private async Task ShareAsync() => await NavigationService.Current.CallDialogAsync<ShareDialog>(Track);

        private void AddToPlaylist(object sender, RoutedEventArgs e) => AddToPlaylistAsync().FireAndForgetSafeAsync();

        private async Task AddToPlaylistAsync() => await NavigationService.Current.CallDialogAsync<AddToPlaylistDialog>(Track);

        /// <summary>
        ///     Toggle if the track has been liked or not
        /// </summary>
        public void ToggleLikeTrack() => ToggleLikeTrackAsync().FireAndForgetSafeAsync();

        public async Task ToggleLikeTrackAsync()
        {
            // Track must exist
            if (Track == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Like Error");
                return;
            }

            // User is not logged in
            if (!SoundByteService.Current.IsServiceConnected(Track.ServiceType))
            {
                await NavigationService.Current.CallMessageDialogAsync($"You must connect your {Track.ServiceType} account to do this.", "Like Error");
                return;
            }

            // Toggle like status
            Track.ToggleLike();
        }

        private void ViewArtist(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), Track.User);
        }

        private void EditTags(object sender, RoutedEventArgs e) => EditTagsAsync().FireAndForgetSafeAsync();

        private async Task EditTagsAsync()
        {
            if (Track.ServiceType != ServiceTypes.Local)
            {
                await NavigationService.Current.CallMessageDialogAsync("This action can only be performed on local tracks.");
            }
        }

        private void OpenInExplorer(object sender, RoutedEventArgs e) => OpenInExplorerAsync().FireAndForgetSafeAsync();

        private async Task OpenInExplorerAsync()
        {
            if (Track.ServiceType != ServiceTypes.Local)
            {
                await NavigationService.Current.CallMessageDialogAsync("This action can only be performed on local tracks.");
            }
        }

        private void Delete(object sender, RoutedEventArgs e) => DeleteAsync().FireAndForgetSafeAsync();

        private async Task DeleteAsync()
        {
            if (Track.ServiceType != ServiceTypes.Local)
            {
                await NavigationService.Current.CallMessageDialogAsync("This action can only be performed on local tracks.");
            }
        }

        private void Play(object sender, RoutedEventArgs e) => PlayAsync().FireAndForgetSafeAsync();

        private async Task PlayAsync()
        {
            var tracks = new SoundByteCollection<DummyTrackSource>
            {
                new BaseSoundByteItem(Track)
            };

            // Play
            await BaseViewModel.PlayAllTracksAsync(tracks);
        }

        private void ShowHoverAnimation(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 22.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
               ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.8f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.Offset(0, -3.0f, 250, 0).Start();
            HoverArea.Fade(0.0f, 200).Start();
        }

        private void HideHoverAnimation(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 2.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 8.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
              ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.2f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.Offset(0, 0, 250, 0).Start();
            HoverArea.Fade(0.3f, 200).Start();
        }

        private async void OpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(Track.Link));
        }
    }
}