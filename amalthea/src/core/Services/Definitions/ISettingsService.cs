using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Service used to handle settings and secure storage
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        ///     Get a secure setting
        /// </summary>
        /// <typeparam name="T">The type of setting to get</typeparam>
        /// <param name="key">The key this setting was saved under</param>
        /// <returns>The setting object</returns>
        Task<T> GetSecureAsync<T>(string key);

        /// <summary>
        ///     Get a standard setting
        /// </summary>
        /// <typeparam name="T">The type of setting to get</typeparam>
        /// <param name="key">The key this setting was saved under</param>
        /// <param name="defaultSetting">The default setting if this one does not exist</param>
        /// <returns>The setting object</returns>
        T GetPreference<T>(string key, T defaultSetting);

        /// <summary>
        ///     Save a secure setting
        /// </summary>
        /// <typeparam name="T">The type of setting to set</typeparam>
        /// <param name="key">The key this setting will be saved under</param>
        /// <param name="value">The value to save</param>
        Task SetSecureAsync<T>(string key, T value);

        /// <summary>
        ///     Save a setting
        /// </summary>
        /// <typeparam name="T">The type of setting to set</typeparam>
        /// <param name="key">The key this setting will be saved under</param>
        /// <param name="value">The value to save</param>
        void SetPreference<T>(string key, T value);

        /// <summary>
        ///     Remove a secure setting.
        /// </summary>
        /// <param name="key">The secure setting to remove</param>
        void RemoveSecure(string key);

        /// <summary>
        ///     Remove a specified setting
        /// </summary>
        /// <param name="key">The setting to remove</param>
        void RemovePreference(string key);

        /// <summary>
        ///     Clear all secure settings saved by the application
        /// </summary>
        void ClearAllSecure();

        /// <summary>
        ///     Clear all settings saved by the application.
        /// </summary>
        void ClearAllPreferences();
    }
}