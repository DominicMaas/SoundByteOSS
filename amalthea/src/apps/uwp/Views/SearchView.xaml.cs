using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.UWP.Views
{
    [MvxViewFor(typeof(SearchViewModel))]
    public sealed partial class SearchView : MvxWindowsPage
    {
        public SearchViewModel Vm => (SearchViewModel)ViewModel;

        public SearchView() => InitializeComponent();
    }
}
