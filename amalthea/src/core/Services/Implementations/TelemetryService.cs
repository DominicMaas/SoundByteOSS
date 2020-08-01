using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Implementations
{
    public class TelemetryService : ITelemetryService
    {
        public Task OptOutAsync()
        {
            return Analytics.SetEnabledAsync(false);
        }

        public void TrackEvent(string eventName, Dictionary<string, string>? properties = null)
        {
            Analytics.TrackEvent(eventName, properties);
        }

        public void TrackException(Exception exception, Dictionary<string, string>? properties = null)
        {
            Crashes.TrackError(exception, properties);
        }

        public void TrackPage(string pageName)
        {
            TrackEvent("Page Navigation", new Dictionary<string, string> { { "PageName", pageName } });
        }
    }
}