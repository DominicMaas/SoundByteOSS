using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using SoundByte.App.Android.Services;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.App.Android
{
    /// <summary>
    ///     Custom setup class to register platform specific services
    /// </summary>
    public class Setup : MvxAppCompatSetup<Core.App>
    {
        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.RegisterSingleton<IStoreService>(new StoreService());
            Mvx.IoCProvider.RegisterSingleton<IDialogService>(new DialogService());
            Mvx.IoCProvider.RegisterSingleton<IPlaybackService>(new PlaybackService());
            base.InitializeFirstChance();
        }
    }
}