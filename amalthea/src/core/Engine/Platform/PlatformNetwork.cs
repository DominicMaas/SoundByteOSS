using Flurl.Http;
using Jint.Native;
using MvvmCross.Logging;
using SoundByte.Core.Extensions;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.Core.Engine.Platform
{
    public class PlatformNetwork
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly MusicProvider _musicProvider;
        private readonly IMvxLog _log;

        public PlatformNetwork(IMvxLogProvider logProvider, IAuthenticationService authenticationService, MusicProvider musicProvider)
        {
            _authenticationService = authenticationService;
            _log = logProvider.GetLogFor<PlatformNetwork>();
            _musicProvider = musicProvider;
        }

        public JsValue PerformRequest(string url)
        {
            _log.Info($"Music provider ({_musicProvider.Manifest.Name}) is requesting a url: {url}");
            return AsyncHelper.RunSync(async () =>
            {
                var content = await url.WithHeader("User-Agent", "SoundByte App")
                    .HandleAuthExpireAsync(_authenticationService, _musicProvider);

                return content;
            });
        }

        public JsValue PerformAnonymousRequest(string url)
        {
            _log.Info($"Music provider ({_musicProvider.Manifest.Name}) is requesting an anonymous url: {url}");
            return AsyncHelper.RunSync(() => url.WithHeader("User-Agent", "SoundByte App").GetStringAsync());
        }
    }
}