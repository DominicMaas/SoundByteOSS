using Microsoft.AppCenter.Crashes;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using SoundByte.Core.Messages;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.ViewModels.Panes;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.ViewModels.Main
{
    /// <summary>
    ///     Only used on the mobile applications (desktop applications
    ///     split this functionality across the main screen)
    /// </summary>
    public class MeViewModel : MvxNavigationViewModel
    {
        #region Services

        private readonly IMvxMessenger _mvxMessenger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStoreService _storeService;

        #endregion Services

        #region Commands

        /// <summary>
        ///     Open the music provider accounts page
        /// </summary>
        public IMvxAsyncCommand NavigateMusicProviderAccountsCommand { get; }

        /// <summary>
        ///     Toggles the users SoundByte account between
        ///     connected and disconnected
        /// </summary>
        public IMvxAsyncCommand ToggleSoundByteAccountCommand { get; }

        /// <summary>
        ///     Opens the settings page
        /// </summary>
        public IMvxAsyncCommand NavigateSettingsCommand { get; }

        /// <summary>
        ///     Opens a page with a list of the users music providers
        /// </summary>
        public IMvxAsyncCommand NavigateInstalledMusicProviderCommand { get; }

        /// <summary>
        ///     Opens a page where the user can search for new music providers
        /// </summary>
        public IMvxAsyncCommand NavigateBrowseMusicProviderCommand { get; }

        /// <summary>
        ///     Purchase SoundByte Premium
        /// </summary>
        public IMvxAsyncCommand NavigateManagePremiumCommand { get; }

        /// <summary>
        ///     Open a website allowing the user to send feedback
        /// </summary>
        public IMvxAsyncCommand SendFeedbackCommand { get; }

        /// <summary>
        ///     Open the review dialog so the user can review the
        ///     application.
        /// </summary>
        public IMvxAsyncCommand OpenReviewDialogCommand { get; }

        /// <summary>
        ///     Open the app changelog in the users browser
        /// </summary>
        public IMvxAsyncCommand OpenChangelogCommand { get; }

        /// <summary>
        ///     Open the twitter page within the twitter app
        /// </summary>
        public IMvxAsyncCommand OpenTwitterCommand { get; }

        /// <summary>
        ///     Open the privacy policy website in the users default browser
        /// </summary>
        public IMvxAsyncCommand OpenPrivacyPolicyCommand { get; }

        /// <summary>
        ///     Open the SoundByte website in the users default browser
        /// </summary>
        public IMvxAsyncCommand OpenWebsiteCommand { get; }

        /// <summary>
        ///     Load an external music provider into SoundByte
        /// </summary>
        public IMvxAsyncCommand<string> LoadMusicProviderCommand;

        #endregion Commands

        #region Getters & Setters

        /// <summary>
        ///     The version of the app
        /// </summary>
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        private string _version;

        /// <summary>
        ///     The build date of this version
        /// </summary>
        public string BuildDate
        {
            get => _buildDate;
            set => SetProperty(ref _buildDate, value);
        }

        private string _buildDate;

        /// <summary>
        ///     The branch that this version  was built from
        /// </summary>
        public string BuildBranch
        {
            get => _buildBranch;
            set => SetProperty(ref _buildBranch, value);
        }

        private string _buildBranch;

        /// <summary>
        ///     The commit this version was built from
        /// </summary>
        public string BuildCommit
        {
            get => _buildCommit;
            set => SetProperty(ref _buildCommit, value);
        }

        private string _buildCommit;

        /// <summary>
        ///     The status of the users SoundByte account
        /// </summary>
        public string SoundByteAccountStatus
        {
            get => _soundByteAccountStatus;
            set => SetProperty(ref _soundByteAccountStatus, value);
        }

        private string _soundByteAccountStatus;

        #endregion Getters & Setters

        public MeViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMvxMessenger mvxMessenger,
            IAuthenticationService authenticationService, IStoreService storeService, IMusicProviderService musicProviderService) : base(logProvider, navigationService)
        {
            // Set services
            _authenticationService = authenticationService;
            _storeService = storeService;
            _mvxMessenger = mvxMessenger;

            // Bind commands
            NavigateMusicProviderAccountsCommand = new MvxAsyncCommand(() => NavigationService.Navigate<AccountsViewModel>());
            NavigateSettingsCommand = new MvxAsyncCommand(() => NavigationService.Navigate<SettingsViewModel>());
            NavigateBrowseMusicProviderCommand = new MvxAsyncCommand(() => NavigationService.Navigate<BrowseMusicProvidersViewModel>());
            NavigateInstalledMusicProviderCommand = new MvxAsyncCommand(() => NavigationService.Navigate<InstalledMusicProvidersViewModel>());
            ToggleSoundByteAccountCommand = new MvxAsyncCommand(ToggleSoundByteAccountAsync);
            OpenReviewDialogCommand = new MvxAsyncCommand(_storeService.RequestReviewAsync);
            NavigateManagePremiumCommand = new MvxAsyncCommand(() => NavigationService.Navigate<PremiumViewModel>());
            LoadMusicProviderCommand = new MvxAsyncCommand<string>(musicProviderService.LoadAsync);

            // Web commands
            OpenWebsiteCommand = new MvxAsyncCommand(() => Browser.OpenAsync(Constants.SoundByteMediaWebsite));
            OpenPrivacyPolicyCommand = new MvxAsyncCommand(() => Browser.OpenAsync($"{Constants.SoundByteMediaWebsite}pages/privacy-policy"));
            OpenTwitterCommand = new MvxAsyncCommand(() => Launcher.OpenAsync("https://twitter.com/SoundByteUWP"));
            OpenChangelogCommand = new MvxAsyncCommand(() => Browser.OpenAsync($"{Constants.SoundByteMediaWebsite}docs/changelog"));
            SendFeedbackCommand = new MvxAsyncCommand(() => Browser.OpenAsync("https://soundbyte.nolt.io"));

            // Messages
            _mvxMessenger.SubscribeOnMainThread<AccountStatusChangeMessage>(async m =>
            {
                await UpdateAccountStateAsync();
            });
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await UpdateAccountStateAsync();

            Version = $"{AppInfo.Version.Major}.{AppInfo.Version.Minor}.{AppInfo.Version.Build}";

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("build_info.json");
                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var serializer = new JsonSerializer();
                var buildInfo = serializer.Deserialize<BuildInformation>(jsonTextReader);

                BuildDate = buildInfo != null ? buildInfo.Time : "Unknown";
                BuildBranch = buildInfo != null ? buildInfo.Branch : "Unknown";
                BuildCommit = buildInfo != null ? buildInfo.Commmit : "Unknown";
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                BuildDate = "Unknown Error";
                BuildBranch = "Unknown Error";
                BuildCommit = "Unknown Error";
            }
        }

        private async Task UpdateAccountStateAsync()
        {
            // Update account status
            var isAccountConnected = await _authenticationService.IsSoundByteAccountConnectedAsync();
            SoundByteAccountStatus = isAccountConnected ? "Disconnect SoundByte Account" : "Connect SoundByte Account";
        }

        private async Task ToggleSoundByteAccountAsync()
        {
            var isAccountConnected = await _authenticationService.IsSoundByteAccountConnectedAsync();
            if (isAccountConnected)
            {
                // Disconnect the account & update UI
                await _authenticationService.DisconnectSoundByteAccountAsync();
                SoundByteAccountStatus = "Connect SoundByte Account";
            }
            else
            {
                await NavigationService.Navigate<AuthenticationViewModel, AuthenticationViewModel.Holder>(new AuthenticationViewModel.Holder(Constants.SoundByteOAuthConnectUrl, Constants.SoundByteOAuthCallbackUrl, null));
            }
        }
    }
}