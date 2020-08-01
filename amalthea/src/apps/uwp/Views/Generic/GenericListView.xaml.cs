using Windows.UI.Xaml.Controls;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Media;
using SoundByte.Core.ViewModels.Generic;

namespace SoundByte.App.UWP.Views.Generic
{
    [MvxViewFor(typeof(GenericListViewModel))]
    public sealed partial class GenericListView : MvxWindowsPage
    {
        public GenericListViewModel Vm => (GenericListViewModel)ViewModel;

        public GenericListView() => InitializeComponent();

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.InvokeCommand.ExecuteAsync(e.ClickedItem as Media);
        }
    }
}
