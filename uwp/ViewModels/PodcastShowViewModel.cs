using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Sources.Podcast;
using SoundByte.App.Uwp.Helpers;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.ViewModels
{
    public class PodcastShowViewModel : BaseViewModel
    {
        #region Model

        public SoundByteCollection<PodcastItemSource> PodcastItems { get; } = new SoundByteCollection<PodcastItemSource>();

        #endregion Model

        #region Getters and Setters

        /// <summary>
        ///     Gets or sets the current podcast show item
        /// </summary>
        public BasePodcast PodcastShow
        {
            get => _podcastShow;
            set
            {
                if (value == _podcastShow) return;

                _podcastShow = value;
                UpdateProperty();
            }
        }

        private BasePodcast _podcastShow;

        #endregion Getters and Setters

        public void Init(BasePodcast show)
        {
            PodcastShow = show;

            PodcastItems.Source.Show = show;
            PodcastItems.RefreshItems();
        }

        /// <summary>
        ///     Shuffles the tracks in the podcast
        /// </summary>
        public async void ShuffleItemsAsync()
        {
            await ShufflePlayAllTracksAsync(PodcastItems);
        }

        public async void PlayPodcast(object sender, ItemClickEventArgs e)
        {
            await PlayAllTracksAsync(PodcastItems, ((BaseSoundByteItem)e.ClickedItem).Track);
        }

        /// <summary>
        ///     Starts playing the playlist
        /// </summary>
        public async void NavigatePlay()
        {
            await PlayAllTracksAsync(PodcastItems);
        }
    }
}