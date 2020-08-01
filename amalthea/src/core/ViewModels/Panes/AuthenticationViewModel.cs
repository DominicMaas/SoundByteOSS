using Flurl.Http;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Services.Definitions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SoundByte.Core.ViewModels.Panes.AuthenticationViewModel;

namespace SoundByte.Core.ViewModels.Panes
{
    public class AuthenticationViewModel : MvxNavigationViewModel<Holder>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;

        public Holder AuthenticationDetails { get; private set; }

        public string StateVerification { get; private set; }

        #region Commands

        public IMvxAsyncCommand CloseCommand { get; private set; }

        public IMvxAsyncCommand<string> HandleRequestCommand { get; private set; }

        #endregion Commands

        public AuthenticationViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService,
            IAuthenticationService authenticationService, IDialogService dialogService) : base(logProvider, navigationService)
        {
            _authenticationService = authenticationService;
            _dialogService = dialogService;

            CloseCommand = new MvxAsyncCommand(async () => await NavigationService.Close(this));
            HandleRequestCommand = new MvxAsyncCommand<string>(HandleRequestAsync);
        }

        public override void Prepare(Holder parameter)
        {
            AuthenticationDetails = parameter;
        }

        private async Task HandleRequestAsync(string url)
        {
            // If starts with redirect url, it's a request we need to parse.
            if (url.StartsWith(AuthenticationDetails.RedirectUrl))
            {
                // Parse the URL for work
                var parser = new QueryParameterCollection(url);

                // First we just check that the state equals (to make sure the url was not hijacked)
                var state = parser.FirstOrDefault(x => x.Key == "state").Value;

                // The state does not match (only if a state is provided
                if ((string.IsNullOrEmpty(state) || state.TrimEnd('#') != StateVerification) && !string.IsNullOrEmpty(StateVerification))
                {
                    // Close the pane
                    await NavigationService.Close(this);

                    // Show the error message
                    await _dialogService.ShowErrorMessageAsync("Login Failed", "State Verification Failed. This could be caused by another process intercepting the SoundByte login procedure. Sign in has been canceled to protect your privacy.");
                    return;
                }

                // We have an error
                if (parser.FirstOrDefault(x => x.Key == "error").Value != null)
                {
                    var type = parser.FirstOrDefault(x => x.Key == "error").Value;
                    var reason = parser.FirstOrDefault(x => x.Key == "error_description").Value;

                    // The user denied the request
                    if (type == "access_denied")
                    {
                        // Close the pane
                        await NavigationService.Close(this);
                        return;
                    }

                    // Close the pane
                    await NavigationService.Close(this);

                    // Show the error message
                    await _dialogService.ShowErrorMessageAsync("Login Failed", reason);
                    return;
                }

                if (parser.FirstOrDefault(x => x.Key == "code").Value == null)
                {
                    // Close the pane
                    await NavigationService.Close(this);

                    // Show the error message
                    await _dialogService.ShowErrorMessageAsync("Login Failed", "No code was found");
                    return;
                }

                // Get the code
                var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                try
                {
                    // Get the login token
                    var loginToken = await _authenticationService.ExchangeCodeForTokenAsync(code, AuthenticationDetails.MusicProviderId);

                    // Connect the service
                    if (AuthenticationDetails.MusicProviderId == null)
                    {
                        // Connect the service
                        await _authenticationService.ConnectSoundByteAccountAsync(loginToken);
                    }
                    else
                    {
                        // Connect the service
                        await _authenticationService.ConnectAccountAsync(AuthenticationDetails.MusicProviderId.Value, loginToken);
                        await _dialogService.ShowInfoMessageAsync("Account Connected", "Your account has been successfully connected");
                    }

                    // Close the pane
                    await NavigationService.Close(this);
                }
                catch (FlurlHttpException fex)
                {
                    var res = await fex.GetResponseStringAsync();
                    await _dialogService.ShowErrorMessageAsync("Login Failed", res);
                    Crashes.TrackError(fex);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    await _dialogService.ShowErrorMessageAsync("Login Failed", ex.Message);
                    Crashes.TrackError(ex);
                }
            }
        }

        public class Holder
        {
            public string ConnectUrl { get; set; }
            public string RedirectUrl { get; set; }
            public Guid? MusicProviderId { get; set; }

            /// <summary>
            ///     Create a new instance of the holder that this view model required
            /// </summary>
            /// <param name="connectUri"></param>
            /// <param name="redirectUri"></param>
            /// <param name="id">Pass in the music provider id to login a music provider, otherwise pass in null to login soundbyte</param>
            public Holder(string connectUri, string redirectUri, Guid? id)
            {
                ConnectUrl = connectUri;
                RedirectUrl = redirectUri;
                MusicProviderId = id;
            }
        }
    }
}