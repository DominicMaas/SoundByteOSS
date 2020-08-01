using Flurl.Http;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;
using SoundByte.Core.Messages;
using SoundByte.Core.Models.Authentication;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISettingsService _settingsService;
        private readonly ITelemetryService _telemetryService;
        private readonly IMvxMessenger _mvxMessenger;

        public AuthenticationService(ISettingsService settingsService, ITelemetryService telemetryService, IMvxMessenger mvxMessenger)
        {
            _settingsService = settingsService;
            _telemetryService = telemetryService;
            _mvxMessenger = mvxMessenger;
        }

        public async Task ConnectAccountAsync(Guid id, Token token)
        {
            _telemetryService.TrackEvent("Connect Music Provider", new Dictionary<string, string> { { "Id", id.ToString() } });
            await _settingsService.SetSecureAsync($"MP-{id}", token);
            _mvxMessenger.Publish(new AccountStatusChangeMessage(this));
        }

        public async Task ConnectSoundByteAccountAsync(Token token)
        {
            _telemetryService.TrackEvent("Connect SoundByte Account");
            await _settingsService.SetSecureAsync($"SoundByte-Account", token);
            _mvxMessenger.Publish(new AccountStatusChangeMessage(this));
        }

        public Task DisconnectAccountAsync(Guid id)
        {
            _telemetryService.TrackEvent("Disconnect Music Provider", new Dictionary<string, string> { { "Id", id.ToString() } });
            _settingsService.RemoveSecure($"MP-{id}");
            _mvxMessenger.Publish(new AccountStatusChangeMessage(this));
            return Task.CompletedTask;
        }

        public Task DisconnectSoundByteAccountAsync()
        {
            _telemetryService.TrackEvent("Disconnect SoundByte Account");
            _settingsService.RemoveSecure($"SoundByte-Account");
            _mvxMessenger.Publish(new AccountStatusChangeMessage(this));
            return Task.CompletedTask;
        }

        public async Task<Token> ExchangeCodeForTokenAsync(string code, Guid? id)
        {
            // If the music provider id is null, this is for SoundByte
            if (id == null)
                id = Constants.SoundByteProviderId;

            // Perform the request
            var request = await $"{Constants.SoundByteMediaWebsite}api/v1/authentication/get-token"
                .PostJsonAsync(new { code, provider = id.ToString() })
                .ReceiveJson<SoundByteAuthHolder>();

            // Something went wrong
            if (!request.Successful)
                throw new Exception(request.ErrorMessage);

            return new Token { AccessToken = request.AccessToken, RefreshToken = request.RefreshToken };
        }

        public Task<Token> GetSoundByteAccountTokenAsync()
        {
            return _settingsService.GetSecureAsync<Token>("SoundByte-Account");
        }

        public Task<Token> GetTokenAsync(Guid id)
        {
            return _settingsService.GetSecureAsync<Token>($"MP-{id}");
        }

        public async Task<bool> IsAccountConnectedAsync(Guid id) => await GetTokenAsync(id) != null;

        public async Task<bool> IsSoundByteAccountConnectedAsync() => await GetSoundByteAccountTokenAsync() != null;

        public async Task<Token> RefreshTokenAsync(string refreshToken, Guid? id)
        {
            // If the music provider id is null, this is for SoundByte
            if (id == null)
                id = Constants.SoundByteProviderId;

            // Perform the request
            var request = await $"{Constants.SoundByteMediaWebsite}api/v1/authentication/refresh-token"
                .PostJsonAsync(new { code = refreshToken, provider = id.ToString() })
                .ReceiveJson<SoundByteAuthHolder>();

            // Something went wrong
            if (!request.Successful)
                throw new Exception(request.ErrorMessage);

            // Sometimes refresh tokens are reused, so they are not returned from above
            if (string.IsNullOrEmpty(request.RefreshToken))
                request.RefreshToken = refreshToken;

            return new Token { AccessToken = request.AccessToken, RefreshToken = request.RefreshToken };
        }

        public class SoundByteAuthHolder
        {
            [JsonProperty("successful")]
            public bool Successful { get; set; }

            [JsonProperty("error_message")]
            public string ErrorMessage { get; set; }

            [JsonProperty("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonProperty("access_token")]
            public string? AccessToken { get; set; }
        }
    }
}