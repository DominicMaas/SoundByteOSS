using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Mac.Core;
using SoundByte.App.macOS.Services;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.macOS
{
    /// <summary>
    ///     Custom setup class to register platform specific services
    /// </summary>
    public class Setup : MvxMacSetup<Core.App>
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