using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace SoundByte.Core.ViewModels.Panes
{
    /// <summary>
    ///     View and manage application settings
    /// </summary>
    public class SettingsViewModel : MvxNavigationViewModel
    {
        public IMvxAsyncCommand CloseCommand { get; private set; }

        public SettingsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            CloseCommand = new MvxAsyncCommand(async () => await NavigationService.Close(this));
        }
    }
}