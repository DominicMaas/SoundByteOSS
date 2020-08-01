using Microsoft.AppCenter.Crashes;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.ObjectModel;

namespace SoundByte.Core.ViewModels.Panes
{
    /// <summary>
    ///     View, manage, install and uninstall music providers
    /// </summary>
    public class InstalledMusicProvidersViewModel : MvxNavigationViewModel
    {
        #region Commands

        /// <summary>
        ///     Close the modal
        /// </summary>
        public IMvxAsyncCommand CloseCommand { get; private set; }

        /// <summary>
        ///     Uninstall a music provider
        /// </summary>
        public IMvxAsyncCommand<MusicProvider> UninstallCommand { get; private set; }

        #endregion Commands

        public ObservableCollection<MusicProvider> MusicProviders { get; }

        public InstalledMusicProvidersViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMusicProviderService musicProviderService, IDialogService dialogService) : base(logProvider, navigationService)
        {
            MusicProviders = musicProviderService.MusicProviders;

            CloseCommand = new MvxAsyncCommand(() => NavigationService.Close(this));
            UninstallCommand = new MvxAsyncCommand<MusicProvider>(async e =>
            {
                try
                {
                    await musicProviderService.UninstallAsync(e.Identifier);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    await dialogService.ShowErrorMessageAsync("Partial error uninstalling music provider", ex.Message);
                }
            });
        }
    }
}