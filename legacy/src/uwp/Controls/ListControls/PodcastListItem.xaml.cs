using SoundByte.Core.Items.Podcast;
using System;
using Windows.System;
using Windows.UI.Xaml;

namespace SoundByte.App.Uwp.Controls.ListControls
{
    public sealed partial class PodcastListItem
    {
        public static readonly DependencyProperty PodcastProperty =
            DependencyProperty.Register(nameof(Podcast), typeof(BasePodcast), typeof(PodcastListItem), null);

        /// <summary>
        ///     Gets or sets the podcast
        /// </summary>
        public BasePodcast Podcast
        {
            get => (BasePodcast)GetValue(PodcastProperty);
            set => SetValue(PodcastProperty, value);
        }

        public PodcastListItem()
        {
            InitializeComponent();
        }

        private async void OpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(Podcast.Link));
        }
    }
}