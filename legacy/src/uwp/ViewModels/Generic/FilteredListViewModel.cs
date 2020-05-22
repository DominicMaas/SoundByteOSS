using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Views;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using System.Linq;

namespace SoundByte.App.Uwp.ViewModels.Generic
{
    /// <summary>
    ///     Filtered view model used on some list view items.
    /// </summary>
    public class FilteredListViewModel : BaseViewModel
    {
        #region Bindings

        /// <summary>
        ///     The title to be displayed on the search page.
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
        ///     The track model to show on this page
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

        /// <summary>
        ///     The list of filters the user can filter by
        /// </summary>
        public List<Filter> Filters
        {
            get => _filters;
            set
            {
                if (value == _filters)
                    return;

                _filters = value;
                UpdateProperty();
            }
        }

        private List<Filter> _filters;

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
            Filters = data.Filters;
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

        public void OnComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the parameters on this source
            Model.Source.ApplyParameters(new Dictionary<string, object>
            {
                { "filter", ((sender as ComboBox)?.SelectedItem as Filter)?.FilterName }
            });

            // Refresh the items
            Model.RefreshItems();
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
            ///     A list of filters that the user can filter by.
            /// </summary>
            public List<Filter> Filters { get; }

            /// <summary>
            ///     Create a new view model
            /// </summary>
            /// <param name="model">The track source</param>
            /// <param name="title">Title to display</param>
            /// <param name="filters">List of filters the user can choose from</param>
            public Holder(SoundByteCollection<ISource> model, string title, Filter[] filters)
            {
                Model = model;
                Title = title;
                Filters = filters.ToList();
            }
        }

        public class Filter
        {
            public Filter(bool isHeader, string displayName)
            {
                IsFilterItem = !isHeader;
                DisplayName = isHeader ? displayName.ToUpper() : displayName;
                FilterName = isHeader ? displayName.ToUpper() : displayName;
            }

            public Filter(string displayName)
            {
                DisplayName = displayName;
                FilterName = displayName;
                IsFilterItem = true;
            }

            public Filter(string displayName, string filterName)
            {
                DisplayName = displayName;
                FilterName = filterName;
                IsFilterItem = true;
            }

            /// <summary>
            ///     The internal filter name (used in the source).
            /// </summary>
            public string FilterName { get; set; }

            /// <summary>
            ///     The display name to show on the UI
            /// </summary>
            public string DisplayName { get; set; }

            public bool IsFilterItem { get; set; }
        }
    }
}