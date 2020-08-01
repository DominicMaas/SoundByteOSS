using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace SoundByte.Core.ViewModels.Panes
{
    /// <summary>
    ///     View premium status and purchase
    /// </summary>
    public class PremiumViewModel : MvxNavigationViewModel
    {
        public IMvxAsyncCommand CloseCommand { get; private set; }

        public PremiumViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            CloseCommand = new MvxAsyncCommand(async () => await NavigationService.Close(this));
        }
    }
}