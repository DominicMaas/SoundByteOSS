using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Views;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.ViewModels.Generic
{
    /// <summary>
    ///     View model used on all list view items.
    /// </summary>
    public class GenericListViewModel : BaseViewModel
    {
        #region Bindings

        /// <summary>
        /// The title to be displayed on the search page.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                    return;

                _title = value;
                UpdateProperty();
            }
        }

        private string _title;

        /// <summary>
        /// The track model to show on this page
        /// </summary>
        public SoundByteCollection<ISource> Model
        {
            get => _model;
            set
            {
                if (value == _model)
                    return;

                _model = value;
                UpdateProperty();
            }
        }

        private SoundByteCollection<ISource> _model;

        #endregion Bindings

        #region Methods

        /// <summary>
        ///     Setup the view model for use
        /// </summary>
        /// <param name="data">The data to use</param>
        public void Init(Holder data)
        {
            Model = data.Model;
            Title = data.Title;
        }

        #endregion Methods

        #region Method Bindings

        public async void PlayShuffleTracks()
        {
            await ShufflePlayAllTracksAsync(Model);
        }

        public async void PlayAllTracks()
        {
            await PlayAllTracksAsync(Model);
        }

        public async void NavigateItem(object sender, ItemClickEventArgs e)
        {
            // Get the clicked item
            var clickedItem = (BaseSoundByteItem)e.ClickedItem;

            switch (clickedItem.Type)
            {
                case ItemType.Track:
                    await PlayAllTracksAsync(Model, clickedItem.Track);
                    break;

                case ItemType.User:
                    App.NavigateTo(typeof(UserView), clickedItem.User);
                    break;

                case ItemType.Playlist:
                    App.NavigateTo(typeof(PlaylistView), clickedItem.Playlist);
                    break;

                case ItemType.Podcast:
                    App.NavigateTo(typeof(PodcastShowView), clickedItem.Podcast);
                    break;
            }
        }

        #endregion Method Bindings

        public class Holder
        {
            /// <summary>
            ///     Track source
            /// </summary>
            public SoundByteCollection<ISource> Model { get; }

            /// <summary>
            ///     Track Title
            /// </summary>
            public string Title { get; }

            /// <summary>
            ///     Create a new view model
            /// </summary>
            /// <param name="model">The track source</param>
            /// <param name="title">Title to display</param>
            public Holder(SoundByteCollection<ISource> model, string title)
            {
                Model = model;
                Title = title;
            }
        }
    }
}