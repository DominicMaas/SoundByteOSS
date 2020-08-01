using Polly;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Models.Sources;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.Sources
{
    public class MusicProviderSource : ISource
    {
        private Dictionary<string, string> _parameters;

        private readonly MusicProvider? _musicProvider;
        private readonly bool _isAuthenticatedFeed;
        private readonly string? _getMethod;

        private readonly IAuthenticationService _authenticationService;

        public MusicProviderSource(MusicProvider? musicProvider, bool isAuthenticatedFeed, string? getMethod, IAuthenticationService authenticationService)
        {
            _musicProvider = musicProvider;
            _isAuthenticatedFeed = isAuthenticatedFeed;
            _getMethod = getMethod;

            _authenticationService = authenticationService;
        }

        public void ApplyParameters(Dictionary<string, string> data) => _parameters = data;

        public Dictionary<string, string> GetParameters() => _parameters;

        public async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default)
        {
            if (_musicProvider == null)
                return new SourceResponse("Missing music provider", $"This music provider is not installed");

            if (string.IsNullOrEmpty(_getMethod))
                return new SourceResponse("Not Supported", $"This music provider does not support this type of request");

            if (_isAuthenticatedFeed && await _authenticationService.IsAccountConnectedAsync(_musicProvider.Identifier) == false)
                return new SourceResponse("Not signed in", $"{_musicProvider.Manifest.Name} requires you to sign in to view this content");

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return new SourceResponse("No Connection", "You are disconnected from the internet");

            // _parameters cannot be null, set to an empty dictionary
            if (_parameters == null) _parameters = new Dictionary<string, string>();

            // Retry in case of exceptions
            return await Task.Run(() => Policy
                .Handle<Exception>()
                .WaitAndRetry(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100))
                .Execute(() => GetContent(count, token)), cancellationToken);
        }

        private SourceResponse GetContent(int count, string token)
        {
            var response = _musicProvider!.CallFunction<SourceResponse>(_getMethod!, count, token, _parameters);
            if (response == null)
                return new SourceResponse("Error running music provider", "The music provider did not return any information. Please contact the music provider author.");

            // Handle not successful
            if (!response.Successful)
                return new SourceResponse(response.MessageTitle, response.MessageContent);

            // Since this code ran within a music provider, we need to set the music provider ids
            foreach (var item in response.Items)
            {
                item.MusicProviderId = _musicProvider.Identifier;

                if (item is Track t)
                    t.User.MusicProviderId = _musicProvider.Identifier;
            }

            return response;
        }
    }
}