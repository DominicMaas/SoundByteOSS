using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls.GridControls
{
    public sealed partial class SoundByteItem
    {
        private long _sourceToken;

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(BaseSoundByteItem), typeof(SoundByteItem), null);

        /// <summary>
        ///     The source for this control
        /// </summary>
        public BaseSoundByteItem Source
        {
            get => (BaseSoundByteItem)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public SoundByteItem()
        {
            InitializeComponent();
            _sourceToken = RegisterPropertyChangedCallback(SourceProperty, SourceChanged);
        }

        private void SourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            switch (Source.Type)
            {
                case ItemType.Playlist:
                    // Only set the content to the playlist if not already set
                    if (!(Content is PlaylistItem) || ((PlaylistItem)Content)?.Playlist?.PlaylistId != Source.Playlist?.PlaylistId)
                        Content = new PlaylistItem { Playlist = Source.Playlist };
                    break;

                case ItemType.Track:
                    // Only set the content to the track if not already set
                    if (!(Content is TrackItem) || ((TrackItem)Content)?.Track?.TrackId != Source.Track?.TrackId)
                        Content = new TrackItem { Track = Source.Track };
                    break;

                case ItemType.User:
                    // Only set the content to the user if not already set
                    if (!(Content is UserItem) || ((UserItem)Content)?.User?.UserId != Source.User?.UserId)
                        Content = new UserItem { User = Source.User };
                    break;

                case ItemType.Podcast:
                    // Only set the content to the podcast if not already set
                    if (!(Content is PodcastItem) || ((PodcastItem)Content)?.Podcast?.PodcastId != Source.Podcast?.PodcastId)
                        Content = new PodcastItem { Podcast = Source.Podcast };
                    break;
            }
        }

        private void SoundByteItem_Unloaded(object sender, RoutedEventArgs e)
        {
            UnregisterPropertyChangedCallback(SourceProperty, _sourceToken);
        }
    }
}