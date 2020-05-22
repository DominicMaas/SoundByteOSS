using SoundByte.Core.Items.Playlist;
using System;
using Windows.System;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls.ListControls
{
    public sealed partial class PlaylistListItem
    {
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(BasePlaylist), typeof(PlaylistListItem), null);

        /// <summary>
        ///     Gets or sets the playlist
        /// </summary>
        public BasePlaylist Playlist
        {
            get => (BasePlaylist)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public PlaylistListItem()
        {
            InitializeComponent();
        }

        private async void OpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(Playlist.Link));
        }
    }
}