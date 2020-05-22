using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.ServicesV2
{
    /// <summary>
    ///     This class handles global app telemetry to all telemetry services
    ///     connected to this app.
    /// </summary>
    public interface ITelemetryService
    {
        /// <summary>
        ///     Opt out of recording telemetry.
        /// </summary>
        Task OptOutAsync();

        /// <summary>
        ///     Track a page/view hit.
        /// </summary>
        /// <param name="pageName">Page/View name</param>
        void TrackPage(string pageName);

        /// <summary>
        ///     Track an event with parameters.
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="properties">Parameters for the event</param>
        void TrackEvent(string eventName, Dictionary<string, string> properties = null);

        /// <summary>
        ///     Track an exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="proerties">Details</param>
        void TrackException(Exception exception, Dictionary<string, string> proerties = null);
    }
}