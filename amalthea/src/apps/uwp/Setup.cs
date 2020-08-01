using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Uap.Core;
using MvvmCross.Platforms.Uap.Presenters;
using MvvmCross.Platforms.Uap.Views;
using SoundByte.App.UWP.Presenters;
using SoundByte.App.UWP.Services;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.UWP
{
    /// <summary>
    ///     Custom setup class to register platform specific services
    /// </summary>
    public class Setup : MvxWindowsSetup<Core.App>
    {
        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStoreService, StoreService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlaybackService, PlaybackService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IActivityService, ActivityService>();
            base.InitializeFirstChance();
        }

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            var customPresenter = new SoundByteViewPresenter(rootFrame);
            Mvx.IoCProvider.RegisterSingleton<MvxWindowsViewPresenter>(customPresenter);
            return customPresenter;
        }
    }
}