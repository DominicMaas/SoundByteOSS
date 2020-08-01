using Newtonsoft.Json;
using SoundByte.Core.Services.Definitions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.Services.Implementations
{
    public class SettingsService : ISettingsService
    {
        public const string AppMode = "AppMode";

        public void ClearAllPreferences()
        {
            Preferences.Clear();
        }

        public void ClearAllSecure()
        {
            SecureStorage.RemoveAll();
        }

        public T GetPreference<T>(string key, T defaultSetting)
        {
            var result = Preferences.Get(key, string.Empty);
            return string.IsNullOrEmpty(result) ? defaultSetting : JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<T> GetSecureAsync<T>(string key)
        {
            var result = await SecureStorage.GetAsync(key);
            if (string.IsNullOrEmpty(result))
                return default;

            return JsonConvert.DeserializeObject<T>(result);
        }

        public void RemovePreference(string key)
        {
            Preferences.Remove(key);
        }

        public void RemoveSecure(string key)
        {
            SecureStorage.Remove(key);
        }

        public void SetPreference<T>(string key, T value)
        {
            Preferences.Set(key, JsonConvert.SerializeObject(value));
        }

        public Task SetSecureAsync<T>(string key, T value)
        {
            return SecureStorage.SetAsync(key, JsonConvert.SerializeObject(value));
        }
    }
}