using MvvmCross.ViewModels;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Sources;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SoundByte.Core.ViewModels.Generic.FilteredListViewModel;

namespace SoundByte.Core.ViewModels.Generic
{
    /// <summary>
    ///     Filtered view model used on some list view items.
    /// </summary>
    public class FilteredListViewModel : MvxViewModel<Holder>
    {
        #region Getters and Setters

        /// <summary>
        ///     The title to display on the list
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _title;

        /// <summary>
        ///     The collection of items
        /// </summary>
        public SoundByteCollection<ISource> Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private SoundByteCollection<ISource> _model;

        #endregion Getters and Setters

        public override void Prepare(Holder parameter)
        {
            Title = parameter.Title;
            Model = parameter.Model;
        }

        public override async Task Initialize()
        {
            await Model.LoadMoreItemsAsync(25, CancellationToken.None);
        }

        public class Holder
        {
            /// <summary>
            ///     Item source
            /// </summary>
            public SoundByteCollection<ISource> Model { get; }

            /// <summary>
            ///     Title
            /// </summary>
            public string Title { get; }

            /// <summary>
            ///     A list of filters that the user can filter by.
            /// </summary>
            public List<Filter> Filters { get; }

            /// <summary>
            ///     Create a new view model
            /// </summary>
            /// <param name="model">The item source</param>
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