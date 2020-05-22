using SoundByte.App.Uwp.Services;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    ///     Static methods for detecting device
    /// </summary>
    public static class DeviceHelper
    {
        /// <summary>
        ///     Is the app running on xbox
        /// </summary>
        public static bool IsXbox => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox";

        /// <summary>
        ///     Is the app running on hololens
        /// </summary>
        public static bool IsHolographic => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Holographic";

        /// <summary>
        ///     Is the app running on team
        /// </summary>
        public static bool IsTeam => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Team";

        /// <summary>
        ///     Is the app runnning on a phone
        /// </summary>
        public static bool IsMobile => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        /// <summary>
        ///     Is the app running on desktop
        /// </summary>
        public static bool IsDesktop => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";

        /// <summary>
        ///     Is the application fullscreen.
        /// </summary>
        public static bool IsDeviceFullScreen => ApplicationView.GetForCurrentView().IsFullScreenMode;

        /// <summary>
        ///     If the app running in debug mode
        /// </summary>
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        ///     Is the app currently in the background.
        ///     Uses apps in built settings to share this state
        ///     between UI/Background threads.
        /// </summary>
        public static bool IsBackground
        {
            get => SettingsService.Instance.IsAppBackground;
            set => SettingsService.Instance.IsAppBackground = value;
        }
    }
}