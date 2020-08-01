using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using SoundByte.Core.Messages;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.ViewModels.Panes
{
    /// <summary>
    ///     Manage music provider accounts
    /// </summary>
    public class AccountsViewModel : MvxNavigationViewModel
    {
        private readonly IMusicProviderService _musicProviderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMvxMessenger _mvxMessenger;

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand<MusicProviderAccount> InvokeCommand { get; }

        public ObservableCollection<MusicProviderAccount> MusicProviderAccounts { get; } = new ObservableCollection<MusicProviderAccount>();

        public AccountsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMusicProviderService musicProviderService,
            IAuthenticationService authenticationService, IMvxMessenger mvxMessenger) : base(logProvider, navigationService)
        {
            _musicProviderService = musicProviderService;
            _authenticationService = authenticationService;
            _mvxMessenger = mvxMessenger;

            CloseCommand = new MvxAsyncCommand(async () => await NavigationService.Close(this));
            InvokeCommand = new MvxAsyncCommand<MusicProviderAccount>(OnAccountMusicProviderInvoke);
        }

        public override async Task Initialize()
        {
            await LoadItems();

            // Listen to any account changes to refresh the user interface
            _mvxMessenger.SubscribeOnMainThread<AccountStatusChangeMessage>(async m =>
            {
                await LoadItems();
            });
        }

        private async Task LoadItems()
        {
            MusicProviderAccounts.Clear();

            // Load music providers that have an auth definition
            foreach (var musicProvider in _musicProviderService.MusicProviders.Where(x => x.Manifest.Authentication != null))
            {
                var musicProviderAccount = new MusicProviderAccount(musicProvider.Identifier, musicProvider.Manifest.Name);

                // Check if the account is connected
                var isConnected = await _authenticationService.IsAccountConnectedAsync(musicProvider.Identifier);
                musicProviderAccount.ConnectedStatus = isConnected ? "Disconnect account" : "Connect account";
                musicProviderAccount.IsConnected = isConnected;

                MusicProviderAccounts.Add(musicProviderAccount);
            }
        }

        private async Task OnAccountMusicProviderInvoke(MusicProviderAccount account)
        {
            var musicProvider = _musicProviderService.MusicProviders.First(x => x.Identifier == account.Id);
            if (account.IsConnected)
            {
                await _authenticationService.DisconnectAccountAsync(account.Id);
            }
            else
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<AuthenticationViewModel, AuthenticationViewModel.Holder>(new AuthenticationViewModel.Holder(musicProvider.Manifest.Authentication.ConnectUrl, musicProvider.Manifest.Authentication.RedirectUrl, account.Id));
            }
        }
    }
}