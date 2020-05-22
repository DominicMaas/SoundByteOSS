using GoogleAnalytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;

namespace SoundByte.App.Uwp.ServicesV2.Implementations
{
    /// <summary>
    ///     This class handles global application telemetry to all telemetry services
    ///     connected to this application.
    /// </summary>
    public class TelemetryService : ITelemetryService
    {
        private readonly Tracker _googleAnalyticsClient;

        public TelemetryService()
        {
            // Used for general analytics
            _googleAnalyticsClient = AnalyticsManager.Current.CreateTracker(AppKeys.GoogleAnalyticsUWPTrackerId);

            // Used for general analytics, push notifications and crashes
            AppCenter.Start(AppKeys.AppCenterUWPClientId, typeof(Analytics), typeof(Push), typeof(Crashes));

            // Set country code
            AppCenter.SetCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);

            // Set application details (used for targeting notifications. Most important is application version for example).
            var properties = new CustomProperties();
            properties.Set("AppVersion", string.Format("{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build));
            properties.Set("WindowsVersion", SystemInformation.OperatingSystemVersion.ToString());
            properties.Set("DevicePlatform", AnalyticsInfo.VersionInfo.DeviceFamily);

            AppCenter.SetCustomProperties(properties);
        }

        public async Task OptOutAsync()
        {
            AnalyticsManager.Current.IsDebug = true;
            AnalyticsManager.Current.AppOptOut = true;
            await Analytics.SetEnabledAsync(false);
        }

        public void TrackPage(string pageName)
        {
            try
            {
                TrackEvent("Page Navigation", new Dictionary<string, string> { { "PageName", pageName } });

                _googleAnalyticsClient.ScreenName = pageName;
                _googleAnalyticsClient.Send(HitBuilder.CreateScreenView().Build());
            }
            catch
            {
                // ignored
            }
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            try
            {
                Analytics.TrackEvent(eventName, properties);
                _googleAnalyticsClient.Send(HitBuilder.CreateCustomEvent("App", "Action", eventName).Build());
            }
            catch
            {
                // ignored
            }
        }

        public void TrackException(Exception exception, Dictionary<string, string> properties = null)
        {
            try
            {
                Analytics.TrackEvent("Exception", new Dictionary<string, string>
                {
                    { "Message", exception.Message },
                    { "StackTrace", exception.StackTrace }
                });

                _googleAnalyticsClient.Send(HitBuilder.CreateException(exception.Message + " : " + exception.StackTrace, false).Build());
            }
            catch
            {
                // ignored
            }
        }
    }
}