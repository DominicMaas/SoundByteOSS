using SoundByte.Core.Helpers;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.Core.Engine.Platform
{
    public class PlatformSettings
    {
        private readonly ISettingsService _settingsService;
        private readonly MusicProvider _musicProvider;

        public PlatformSettings(ISettingsService settingsService, MusicProvider _musicProvider)
        {
            _settingsService = settingsService;
            this._musicProvider = _musicProvider;
        }

        public string GetPreference(string key, string defaultValue)
        {
            return _settingsService.GetPreference($"MP-{_musicProvider.Identifier}-{key}", defaultValue);
        }

        public string GetSecure(string key)
        {
            return AsyncHelper.RunSync(() => _settingsService.GetSecureAsync<string>($"MP-{_musicProvider.Identifier}-{key}"));
        }

        public void RemovePreference(string key)
        {
            _settingsService.RemovePreference($"MP-{_musicProvider.Identifier}-{key}");
        }

        public void RemoveSecure(string key)
        {
            _settingsService.RemoveSecure($"MP-{_musicProvider.Identifier}-{key}");
        }

        public void SetPreference(string key, string value)
        {
            _settingsService.SetPreference($"MP-{_musicProvider.Identifier}-{key}", value);
        }

        public void SetSecure(string key, string value)
        {
            AsyncHelper.RunSync(() => _settingsService.SetSecureAsync($"MP-{_musicProvider.Identifier}-{key}", value));
        }
    }
}