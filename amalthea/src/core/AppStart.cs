using MonkeyCache.LiteDB;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.ViewModels;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core
{
    /// <summary>
    ///     Custom startup logic
    /// </summary>
    public class AppStart : MvxAppStart
    {
        private readonly IMusicProviderService _musicProviderService;
        private readonly IStoreService _storeService;

        public AppStart(IMvxApplication application, IMvxNavigationService navigationService, IMusicProviderService musicProviderService, IStoreService storeService) : base(application, navigationService)
        {
            _musicProviderService = musicProviderService;
            _storeService = storeService;
        }

        protected override async Task NavigateToFirstViewModel(object? hint = null)
        {
            // Track versions
            VersionTracking.Track();

            // Setup Cache
            Barrel.ApplicationId = "soundbyte_cache";

            // Ensure premium is loaded
            await _storeService.InitializeAsync();

            // Load any application music providers
            await _musicProviderService.FindAndLoadAsync();

            // Check for updates and update these providers
            await _musicProviderService.CheckForUpdatesAndInstallAsync();

            // If this is the first time the user has ever opened the application
            var firstLaunch = VersionTracking.IsFirstLaunchEver;

            // On first launch, run the tutorial
            if (firstLaunch)
            {
                //await NavigationService.Navigate<TutorialViewModel>(); // <!-- TODO
                await NavigationService.Navigate<RootViewModel>();
            }
            else
            {
                // Navigate to the root view model
                await NavigationService.Navigate<RootViewModel>();
            }
        }
    }
}