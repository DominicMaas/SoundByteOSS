using Flurl.Http;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Panes
{
    /// <summary>
    ///     Browse for new music providers to install within the application
    /// </summary>
    public class BrowseMusicProvidersViewModel : MvxNavigationViewModel
    {
        /// <summary>
        ///     List of filtered music providers
        /// </summary>
        public ObservableCollection<WebMusicProvider> MusicProviders { get; } = new ObservableCollection<WebMusicProvider>();

        public IMvxAsyncCommand CloseCommand { get; }

        /// <summary>
        ///     Command to install the music provider
        /// </summary>
        public IMvxAsyncCommand<WebMusicProvider> InstallCommand { get; }

        /// <summary>
        ///     Command that triggers the UI to refresh
        /// </summary>
        public IMvxAsyncCommand SearchUpdateCommand { get; }

        /// <summary>
        ///     The current text in the search box
        /// </summary>
        public string? SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string? _searchText;

        public BrowseMusicProvidersViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMusicProviderService musicProviderService) : base(logProvider, navigationService)
        {
            CloseCommand = new MvxAsyncCommand(async () => await NavigationService.Close(this));
            InstallCommand = new MvxAsyncCommand<WebMusicProvider>(async m => { await musicProviderService.InstallAsync(m.Id); });
            SearchUpdateCommand = new MvxAsyncCommand(async () => await UpdateListAsync());
        }

        public override async Task Initialize()
        {
            await UpdateListAsync();
        }

        private async Task UpdateListAsync()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                var musicProviders = await $"{Constants.SoundByteMediaWebsite}api/v1/music-providers".GetJsonAsync<List<WebMusicProvider>>();
                MusicProviders.Clear();
                foreach (var provider in musicProviders)
                {
                    MusicProviders.Add(provider);
                }
            }
            else
            {
                var musicProviders = await $"{Constants.SoundByteMediaWebsite}api/v1/music-providers?searchQuery={SearchText}".GetJsonAsync<List<WebMusicProvider>>();
                MusicProviders.Clear();
                foreach (var provider in musicProviders)
                {
                    MusicProviders.Add(provider);
                }
            }
        }
    }
}