using SoundByte.App.Uwp.ViewModels.Generic;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views.Generic
{
    public sealed partial class UserListView
    {
        public GenericListViewModel ViewModel { get; } = new GenericListViewModel();

        public UserListView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((GenericListViewModel.Holder)e.Parameter);
        }
    }
}