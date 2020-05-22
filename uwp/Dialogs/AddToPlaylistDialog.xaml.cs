using JetBrains.Annotations;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Dialogs
{
    /// <summary>
    ///     Allows the user to add and remove items to and from
    ///     playlists.
    /// </summary>
    public sealed partial class AddToPlaylistDialog
    {
        // Stop the check event when loading
        private bool _blockItemsLoading;

        public AddToPlaylistDialog(BaseTrack trackItem)
        {
            // Do this before the xaml is loaded, to make sure
            // the object can be binded to.
            Track = trackItem;

            // Load the XAML page
            InitializeComponent();

            // Bind the open event handler
            Opened += LoadContent;
        }

        /// <summary>
        ///     The track that we want to add to a playlist
        /// </summary>
        public BaseTrack Track { get; }

        /// <summary>
        ///     A list of user playlists that we can add
        ///     this track to.
        /// </summary>
        private ObservableCollection<BasePlaylist> Playlist { get; } = new ObservableCollection<BasePlaylist>();

        public async void CreatePlaylist()
        {
            if (Track != null)
            {
                // Hide the current dialog
                Hide();

                var result = await new CreatePlaylistDialog(Track.ServiceType).ShowAsync();
                if (result == ContentDialogResult.Primary ||
                    result == ContentDialogResult.Secondary ||
                    result == ContentDialogResult.None)
                {
                    await ShowAsync();
                }
            }
            else
            {
                await NavigationService.Current.CallMessageDialogAsync("No track selected. Cannot load playlists");
            }
        }

        private async void LoadContent(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (Track == null)
                return;

            if (!SoundByteService.Current.IsServiceConnected(Track.ServiceType))
            {
                Hide();
                await NavigationService.Current.CallMessageDialogAsync("You must first login to add tracks to playlists.", "Add Playlist Error");
                return;
            }

            // We are loading content
            LoadingRing.Visibility = Visibility.Visible;

            _blockItemsLoading = true;

            try
            {
                var playlists = new List<BasePlaylist>();
                switch (Track.ServiceType)
                {
                    case ServiceTypes.SoundCloud:
                    case ServiceTypes.SoundCloudV2:
                        playlists.AddRange((await SoundByteService.Current.GetAsync<List<SoundCloudPlaylist>>(ServiceTypes.SoundCloud, "/me/playlists")).Response.Select(x => x.ToBasePlaylist()));
                        break;

                    case ServiceTypes.YouTube:
                        playlists.AddRange((await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(
                            ServiceTypes.YouTube, "playlists",
                            new Dictionary<string, string>
                            {
                                {"part", "id,snippet,contentDetails"},
                                {"maxResults", "50"},
                                {"mine", "true"}
                            })).Response.Playlists.Select(x => x.ToBasePlaylist()));
                        break;

                    default:
                        await new MessageDialog("Playlist support is not enabled for " + Track.ServiceType, "Add Playlist Error").ShowAsync();
                        _blockItemsLoading = false;
                        LoadingRing.Visibility = Visibility.Collapsed;
                        return;
                }

                // Clear UI list
                Playlist.Clear();

                // Loop though all the playlists
                foreach (var playlist in playlists)
                {
                    _blockItemsLoading = true;

                    try
                    {
                        var playlistItemSource = new GenericPlaylistItemSource
                        {
                            PlaylistId = playlist.PlaylistId,
                            Service = playlist.ServiceType
                        };

                        var response = await playlistItemSource.GetItemsAsync(50, string.Empty);

                        if (response.IsSuccess)
                        {
                            // Check if the track in in the playlist
                            playlist.IsTrackInInternalSet = response.Items.FirstOrDefault(x => x.Track.TrackId == Track.TrackId) != null;
                        }
                    }
                    catch
                    {
                        await NavigationService.Current.CallMessageDialogAsync(
                            $"Could not load tracks for this playlist. ({playlist.Title})");
                    }

                    // Add the track to the UI
                    Playlist.Add(playlist);

                    _blockItemsLoading = false;
                }
            }
            catch (SoundByteException ex)
            {
                await NavigationService.Current.CallMessageDialogAsync(ex.ErrorDescription, ex.ErrorTitle);
            }
            finally
            {
                _blockItemsLoading = false;

                // We are done loading content
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     This method is called whenever the playlist items checkbox is unchecked
        ///     This method then removes the currently playing track from the playlist
        ///     and updates the UI.
        /// </summary>
        private async void RemoveTrackFromPlaylist(object sender, RoutedEventArgs e)
        {
            // Used to stop the playlist object running on first load
            if (_blockItemsLoading) return;

            // Show the loading ring to let the user know that we are doing something
            LoadingRing.Visibility = Visibility.Visible;

            // Attempt to remove from playlist
            var addToPlaylist = await ((BasePlaylist)((CheckBox)e.OriginalSource).DataContext).RemoveTrackAsync(Track);
            if (!addToPlaylist.success)
            {
                // Uncheck the box
                _blockItemsLoading = true;
                ((CheckBox)e.OriginalSource).IsChecked = true;
                _blockItemsLoading = false;

                // Alert the user that the request failed, also alert the reason
                await NavigationService.Current.CallMessageDialogAsync("An error occured while trying to remove the track from this playlist.\n" + addToPlaylist.errorMessage);
            }

            // Not loading anymore
            LoadingRing.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     This method opens a dialog box for the user to
        ///     create a playlist and add the current track to it.
        /// </summary>
        private async void AddTrackToPlaylist(object sender, RoutedEventArgs e)
        {
            // Used to stop the playlist object running on first load
            if (_blockItemsLoading) return;

            // Show the loading ring to let the user know that we are doing something
            LoadingRing.Visibility = Visibility.Visible;

            // Attempt to add to playlist
            var addToPlaylist = await ((BasePlaylist)((CheckBox)e.OriginalSource).DataContext).AddTrackAsync(Track);
            if (!addToPlaylist.success)
            {
                // Uncheck the box
                _blockItemsLoading = true;
                ((CheckBox)e.OriginalSource).IsChecked = false;
                _blockItemsLoading = false;

                // Alert the user that the request failed, also alert the reason
                await NavigationService.Current.CallMessageDialogAsync("An error occured while trying to add the track to this playlist.\n" + addToPlaylist.errorMessage);
            }

            // Not loading anymore
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
}