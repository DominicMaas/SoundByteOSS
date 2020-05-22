using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.Views;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.App.Uwp.Controls.GridControls
{
    public sealed partial class PlaylistItem
    {
        /// <summary>
        /// Identifies the Playlist dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(BasePlaylist), typeof(PlaylistItem), null);

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public BasePlaylist Playlist
        {
            get => (BasePlaylist)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public PlaylistItem() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var gridViewItemElement = this.FindAscendant<GridViewItem>();
            if (gridViewItemElement != null)
            {
                gridViewItemElement.GotFocus += GridViewItemElement_GotFocus;
                gridViewItemElement.LostFocus += GridViewItemElement_LostFocus;
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

        private void ViewArtist(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), Playlist.User);
        }

        private void ShufflePlay(object sender, RoutedEventArgs e) => ShufflePlayAsync().FireAndForgetSafeAsync();

        private async Task ShufflePlayAsync()
        {
            // Get the tracks
            var playlistTracks = new SoundByteCollection<GenericPlaylistItemSource>();
            playlistTracks.Source.PlaylistId = Playlist.PlaylistId;
            playlistTracks.Source.Service = Playlist.ServiceType;
            playlistTracks.RefreshItems();

            // Shuffle Play
            await BaseViewModel.ShufflePlayAllTracksAsync(playlistTracks);
        }

        private void Play(object sender, RoutedEventArgs e) => PlayAsync().FireAndForgetSafeAsync();

        private async Task PlayAsync()
        {
            // Get the tracks
            var playlistTracks = new SoundByteCollection<GenericPlaylistItemSource>();
            playlistTracks.Source.PlaylistId = Playlist.PlaylistId;
            playlistTracks.Source.Service = Playlist.ServiceType;
            playlistTracks.RefreshItems();

            // Play
            await BaseViewModel.PlayAllTracksAsync(playlistTracks);
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
            await Launcher.LaunchUriAsync(new Uri(Playlist.Link));
        }
    }
}