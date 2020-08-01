using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Panes;

namespace SoundByte.App.UWP.Views.Panes
{
    [SoundByteModal("Settings")]
    [MvxViewFor(typeof(SettingsViewModel))]
    public sealed partial class SettingsView : MvxWindowsPage
    {
        public SettingsViewModel Vm => (SettingsViewModel)ViewModel;

        public SettingsView() => InitializeComponent();
    }
}
