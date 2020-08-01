using MvvmCross;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels.Main;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Dialogs
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public MeViewModel ViewModel { get; }

        public AboutDialog()
        {
            // Load the viewModel
            var viewModelLoader = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>();
            ViewModel = (MeViewModel)viewModelLoader.LoadViewModel(new MvxViewModelRequest<MeViewModel>(), null);

            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}