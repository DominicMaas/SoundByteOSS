using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls.ListControls
{
    public sealed partial class SoundByteListItem
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(BaseSoundByteItem), typeof(SoundByteListItem), null);

        /// <summary>
        ///     The source for this control
        /// </summary>
        public BaseSoundByteItem Source
        {
            get => (BaseSoundByteItem)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public SoundByteListItem()
        {
            InitializeComponent();
            DataContextChanged += SoundByteListItem_DataContextChanged;
        }

        private void SoundByteListItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Source == null)
                return;

            switch (Source.Type)
            {
                case ItemType.Playlist:
                    // Only set the content to the playlist if not already set
                    if (!(Content is PlaylistListItem) || ((PlaylistListItem)Content)?.Playlist?.PlaylistId != Source.Playlist?.PlaylistId)
                        Content = new PlaylistListItem { Playlist = Source.Playlist };
                    break;

                case ItemType.Track:
                    // Only set the content to the track if not already set
                    if (!(Content is TrackListItem) || ((TrackListItem)Content)?.Track?.TrackId != Source.Track?.TrackId)
                        Content = new TrackListItem { Track = Source.Track };
                    break;

                case ItemType.User:
                    // Only set the content to the user if not already set
                    if (!(Content is UserListItem) || ((UserListItem)Content)?.User?.UserId != Source.User?.UserId)
                        Content = new UserListItem { User = Source.User };
                    break;

                case ItemType.Podcast:
                    // Only set the content to the podcast if not already set
                    if (!(Content is PodcastListItem) || ((PodcastListItem)Content)?.Podcast?.PodcastId != Source.Podcast?.PodcastId)
                        Content = new PodcastListItem { Podcast = Source.Podcast };
                    break;
            }
        }
    }
}