using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels.Generic;

namespace SoundByte.App.UWP.Views.Generic
{
    [MvxViewFor(typeof(FilteredListViewModel))]
    public sealed partial class FilteredListView : MvxWindowsPage
    {
        public FilteredListViewModel Vm => (FilteredListViewModel)ViewModel;

        public FilteredListView() => InitializeComponent();
    }
}
