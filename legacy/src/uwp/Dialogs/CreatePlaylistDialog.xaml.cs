using SoundByte.Core.Items.Playlist;
using System;
using Windows.UI.Popups;

namespace SoundByte.App.Uwp.Dialogs
{
    public sealed partial class CreatePlaylistDialog
    {
        private int _serviceType;

        public CreatePlaylistDialog(int serviceType)
        {
            _serviceType = serviceType;
            InitializeComponent();
        }

        private async void CreatePlaylist(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PlaylistTitle.Text))
            {
                await new MessageDialog("Please enter a playlist name.", "Playlist Creation Error").ShowAsync();
                return;
            }

            try
            {
                await BasePlaylist.CreatePlaylistAsync(_serviceType, PlaylistTitle.Text, IsPrivate.IsOn);
                Hide();
            }
            catch (Exception ex)
            {
                await new MessageDialog("Could not create playlist. Please try again later.\nReason: " + ex.Message, "Playlist Creation Error").ShowAsync();
            }
        }
    }
}