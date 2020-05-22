using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using WinRTXamlToolkit.Tools;

namespace SoundByte.App.Uwp.Services
{
    /// <summary>
    ///     This class handles all the settings within the app
    /// </summary>
    public class SettingsService : INotifyPropertyChanged
    {
        #region Static Class Setup

        private static readonly Lazy<SettingsService> InstanceHolder =
            new Lazy<SettingsService>(() => new SettingsService());

        public static SettingsService Instance => InstanceHolder.Value;

        #endregion Static Class Setup

        #region Getter and Setters

        /// <summary>
        ///     If app preview features are enabled
        /// </summary>
        public bool PreviewFeaturesEnabled
        {
            get => ReadSettingsValue() as bool? ?? false;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     If the app is in interactive mode (Xbox UI on desktop)
        /// </summary>
        public bool IsInteractiveMode
        {
            get => ReadSettingsValue(true) as bool? ?? false;
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     If the user can see the rating dialog, this is disable once
        ///     the user manually rates (via about) or clicks the dismiss dialog.
        /// </summary>
        public bool EnableRating
        {
            get => ReadSettingsValue(true) as bool? ?? true;
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     If the app can show notifications
        /// </summary>
        public bool EnableNotifications
        {
            get => ReadSettingsValue() as bool? ?? true;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     Disable video playback on the 'now playing' view.
        /// </summary>
        public bool DisableVideoPlayback
        {
            get => ReadSettingsValue() as bool? ?? false;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     If the last played song syncs accross devices
        /// </summary>
        public bool SyncLastPlayed
        {
            get => ReadSettingsValue() as bool? ?? true;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     Is windows timeline support enabled.
        /// </summary>
        public bool WindowsTimelineEnabled
        {
            get => ReadSettingsValue() as bool? ?? true;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///  Get the current session ID
        /// </summary>
        public string SessionId
        {
            get
            {
                if (string.IsNullOrEmpty(_sessionId))
                    _sessionId = Guid.NewGuid().ToString().Replace("-", "");

                return _sessionId;
            }
            set => _sessionId = value;
        }

        private string _sessionId;

        /// <summary>
        ///     The amount of track blur
        /// </summary>
        public double TrackBlur
        {
            get => ReadSettingsValue() as double? ?? 0;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     Last playback volume
        /// </summary>
        public double PlaybackVolume
        {
            get => ReadSettingsValue() as double? ?? 100.0;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     Is the app currently using the default system theme
        /// </summary>
        public bool IsDefaultTheme
        {
            get
            {
                switch (ApplicationThemeType)
                {
                    case AppTheme.Default:
                        return true;

                    case AppTheme.Dark:
                        return false;

                    case AppTheme.Light:
                        return false;

                    default:
                        return true;
                }
            }
        }

        /// <summary>
        ///     The apps currently picked theme color
        /// </summary>
        public ApplicationTheme ThemeType
        {
            get
            {
                switch (ApplicationThemeType)
                {
                    case AppTheme.Dark:
                        return ApplicationTheme.Dark;

                    case AppTheme.Light:
                        return ApplicationTheme.Light;

                    case AppTheme.Default:
                        return ApplicationTheme.Dark;

                    default:
                        return ApplicationTheme.Dark;
                }
            }
        }

        public bool IsAppBackground
        {
            get
            {
                var boolVal = ReadSettingsValue() as bool?;
                return boolVal.HasValue && boolVal.Value;
            }
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     The last stored app version
        /// </summary>
        public string AppStoredVersion
        {
            get => ReadSettingsValue(true) as string;
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     App ID Generated by the server.
        /// </summary>
        public string AppId
        {
            get => ReadSettingsValue(true) as string;
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     The user saved language for the app
        /// </summary>
        public string CurrentAppLanguage
        {
            get => ReadSettingsValue(true) as string;
            set => SaveSettingsValue(value);
        }

        /// <summary>
        ///     The page that the app should start on
        /// </summary>
        public string StartPage
        {
            get => ReadSettingsValue() as string;
            set => SaveSettingsValue(value, true);
        }

        /// <summary>
        ///     Gets the application theme type
        /// </summary>
        public AppTheme ApplicationThemeType
        {
            get
            {
                var stringVal = ReadSettingsValue() as string;

                if (string.IsNullOrEmpty(stringVal))
                    return AppTheme.Default;

                try
                {
                    var enumVal = (AppTheme)Enum.Parse(typeof(AppTheme), stringVal);
                    return enumVal;
                }
                catch
                {
                    return AppTheme.Default;
                }
            }
            set => SaveSettingsValue(value.ToString(), true);
        }

        /// <summary>
        ///     Gets if settings syncing is enabled or not
        /// </summary>
        public bool IsSyncSettingsEnabled
        {
            get => ReadBoolSetting(ReadSettingsValue(true) as bool?, true);
            set => SaveSettingsValue(value);
        }

        #endregion Getter and Setters

        #region Settings Helpers

        /// <summary>
        ///     Used to Return bool values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool ReadBoolSetting(bool? value, bool defaultValue)
        {
            return value ?? defaultValue;
        }

        /// <summary>
        ///     Reads a settings value. This method will check the roaming data to see if anything is saved first
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <param name="forceLocal"></param>
        /// <returns>Saved object</returns>
        private object ReadSettingsValue(bool forceLocal = false, [CallerMemberName] string key = "")
        {
            // Check if the force local flag is enabled
            if (forceLocal)
                return GetLocalValue(key);

            // Check if sync is enabled
            if (!IsSyncSettingsEnabled) return GetLocalValue(key);
            // Get remote value
            var remoteValue = GetRemoteValue(key);
            // Return the remote value if it exists
            return remoteValue ?? GetLocalValue(key);
        }

        /// <summary>
        ///     Gets a remote value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetRemoteValue(string key)
        {
            return ApplicationData.Current.RoamingSettings.Values.ContainsKey(key)
                ? ApplicationData.Current.RoamingSettings.Values[key]
                : null;
        }

        /// <summary>
        ///     Gets a local value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetLocalValue(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key)
                ? ApplicationData.Current.LocalSettings.Values[key]
                : null;
        }

        /// <summary>
        ///     Save a key value pair in settings. Create if it doesn't exist
        /// </summary>
        /// <param name="key">Used to find the value at a later state</param>
        /// <param name="value">what to save</param>
        /// <param name="canSync">should this value save online? (If user has enabled syncing)</param>
        private void SaveSettingsValue(object value, bool canSync = false, [CallerMemberName] string key = "")
        {
            // Check if this value supports remote syncing
            if (canSync)
                if (IsSyncSettingsEnabled)
                    if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
                    {
                        // Create new value
                        ApplicationData.Current.RoamingSettings.Values.Add(key, value);
                        return;
                    }
                    else
                    {
                        // Edit existing value
                        ApplicationData.Current.RoamingSettings.Values[key] = value;
                        return;
                    }

            // Store the value locally
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            else
                ApplicationData.Current.LocalSettings.Values[key] = value;

            OnPropertyChanged(key);
        }

        #endregion Settings Helpers

        #region Vault Helpers

        /// <summary>
        ///     Deletes all instances of a certain resource.
        /// </summary>
        /// <param name="resource"></param>
        public void DeleteAllFromVault(string resource)
        {
            try
            {
                var vault = new PasswordVault();
                vault.FindAllByResource(resource).ForEach(x => vault.Remove(x));
            }
            catch
            {
                // Don't do anything, the vault probably does not exist so we don't
                // have to worry.
            }
        }

        #endregion Vault Helpers

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    ///     The possible states for the app theme
    /// </summary>
    public enum AppTheme
    {
        Default,
        Light,
        Dark
    }
}