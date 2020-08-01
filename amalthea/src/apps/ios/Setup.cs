using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using SoundByte.App.iOS.Services;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.iOS
{
    /// <summary>
    ///     Custom setup class to register platform specific services
    /// </summary>
    public class Setup : MvxIosSetup<Core.App>
    {
        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStoreService, StoreService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlaybackService, PlaybackService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
            base.InitializeFirstChance();
        }
    }
}