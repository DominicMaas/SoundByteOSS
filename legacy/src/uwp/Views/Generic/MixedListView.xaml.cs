using SoundByte.App.Uwp.ViewModels.Generic;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Generic
{
    public sealed partial class MixedListView
    {
        public GenericListViewModel ViewModel { get; } = new GenericListViewModel();

        public MixedListView() => InitializeComponent();

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((GenericListViewModel.Holder)e.Parameter);
        }
    }
}