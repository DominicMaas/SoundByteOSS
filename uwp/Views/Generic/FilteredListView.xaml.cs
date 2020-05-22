using SoundByte.App.Uwp.ViewModels.Generic;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Generic
{
    public sealed partial class FilteredListView
    {
        public FilteredListViewModel ViewModel { get; } = new FilteredListViewModel();

        public FilteredListView() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Create the view model
            ViewModel.Init((FilteredListViewModel.Holder)e.Parameter);

            // Initial filter (Select the first in the list
            ViewModel.Model.Source.ApplyParameters(new Dictionary<string, object>
            {
                { "filter", ViewModel.Filters.Find(x => x.IsFilterItem).FilterName }
            });

            // Select the first item
            FilterComboBox.SelectedIndex = ViewModel.Filters.FindIndex(x => x.IsFilterItem);
        }
    }
}