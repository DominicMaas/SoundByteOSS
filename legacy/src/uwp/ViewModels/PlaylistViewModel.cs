using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Helpers;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        public SoundByteCollection<GenericPlaylistItemSource> PlaylistTracks { get; }
            = new SoundByteCollection<GenericPlaylistItemSource>();

        public void SetupView(BasePlaylist newPlaylist)
        {
            // Check if the models saved playlist is null
            if (newPlaylist != null && (Playlist == null || Playlist.PlaylistId != newPlaylist.PlaylistId))
            {
                // Set the item source
                PlaylistTracks.Source.PlaylistId = newPlaylist.PlaylistId;
                PlaylistTracks.Source.Service = newPlaylist.ServiceType;
                PlaylistTracks.RefreshItems();

                // Set the playlist
                Playlist = newPlaylist;

                // Get the resource loader
                var resources = ResourceLoader.GetForCurrentView();

                // Check if the tile is pinned
                if (TileHelper.IsTilePinned("Playlist_" + Playlist.PlaylistId))
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
            }
        }

        #region Private Variables

        // The playlist object
        private BasePlaylist _playlist;

        // Icon for the pin button
        private string _pinButtonIcon = "\uE718";

        // Text for the pin button
        private string _pinButtonText;

        #endregion Private Variables

        #region Model

        /// <summary>
        ///     Gets or sets the current playlist object
        /// </summary>
        public BasePlaylist Playlist
        {
            get => _playlist;
            set
            {
                if (value == _playlist) return;

                _playlist = value;
                UpdateProperty();
            }
        }

        public string PinButtonIcon
        {
            get => _pinButtonIcon;
            set
            {
                if (value != _pinButtonIcon)
                {
                    _pinButtonIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonText
        {
            get => _pinButtonText;
            set
            {
                if (value != _pinButtonText)
                {
                    _pinButtonText = value;
                    UpdateProperty();
                }
            }
        }

        public void Refresh()
        {
            PlaylistTracks.RefreshItems();
        }

        /// <summary>
        ///     Pins or unpins a playlist from the start
        ///     menu / screen.
        /// </summary>
        public async void PinPlaylist()
        {
            // Show the loading ring
            await App.SetLoadingAsync(true);
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();
            // Check if the tile exists
            if (TileHelper.IsTilePinned("Playlist_" + Playlist.PlaylistId))
            {
                // Try remove the tile
                if (await TileHelper.RemoveTileAsync("Playlist_" + Playlist.PlaylistId))
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
            }
            else
            {
                // Create the tile
                if (await TileHelper.CreateTileAsync("Playlist_" + Playlist.PlaylistId, Playlist.Title,
                    "soundbyte://playlist?id=" + Playlist.PlaylistId + "&service=" + Playlist.ServiceType,
                    new Uri(Playlist.ThumbnailUrl), ForegroundText.Light))
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
            }
            // Hide the loading ring
            await App.SetLoadingAsync(false);
        }

        /// <summary>
        ///     Shuffles the tracks in the playlist
        /// </summary>
        public async void ShuffleItemsAsync()
        {
            await ShufflePlayAllTracksAsync(PlaylistTracks);
        }

        /// <summary>
        ///     Called when the user taps on a sound in the
        ///     Sounds tab
        /// </summary>
        public async void TrackClicked(object sender, ItemClickEventArgs e)
        {
            await PlayAllTracksAsync(PlaylistTracks, ((BaseSoundByteItem)e.ClickedItem).Track);
        }

        /// <summary>
        ///     Starts playing the playlist
        /// </summary>
        public async void NavigatePlay()
        {
            await PlayAllTracksAsync(PlaylistTracks);
        }

        #endregion Model
    }
}