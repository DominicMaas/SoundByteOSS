using System;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     Lots of helper function to do with managing
    ///     time, numbers and string formatting
    /// </summary>
    public static class NumberFormatHelper
    {
        /// <summary>
        ///     Converts a large number into a more human
        ///     readable form by adding suffixes. e.g: 6M, 9.3K, 8.2B
        /// </summary>
        /// <param name="number">The number to format</param>
        /// <returns>Formatted String</returns>
        public static string GetFormattedLargeNumber(double number)
        {
            if (number > 1000000000)
                return Math.Round(number / 1000000000, 1) + "B";

            if (number > 100000000)
                return Math.Round(number / 1000000, 0) + "M";

            if (number > 1000000)
                return Math.Round(number / 1000000, 1) + "M";

            if (number > 100000)
                return Math.Round(number / 1000, 0) + "K";

            if (number > 10000)
                return Math.Round(number / 1000, 1) + "K";

            return string.Format("{0:n0}", number);
        }

        /// <summary>
        ///     Formats a timespan into a human
        ///     readable form.
        /// </summary>
        /// <param name="inputMilliseconds">Time to format</param>
        /// <returns>Formatted time string</returns>
        public static string FormatTimeString(double inputMilliseconds)
        {
            // Convert the milliseconds into a usable timespan
            var timeSpan = TimeSpan.FromMilliseconds(inputMilliseconds);

            // Check if the length is less than one minute
            if (timeSpan.TotalMinutes < 1.0)
                return string.Format("{0:D2}:{1:D2}", 0, timeSpan.Seconds);

            return timeSpan.TotalHours < 1.0
                ? string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds)
                : string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        }

        /// <summary>
        ///     Formats a specified date into a friendly
        ///     date time formatted string
        /// </summary>
        /// <param name="date">The date to format</param>
        /// <param name="useShortName">Omit the time</param>
        /// <returns></returns>
        public static string GetTimeDateString(DateTime date, bool useShortName)
        {
            // Convert the date to local time
            date = date.ToLocalTime();
            // The current Date Time
            var currentDateTime = DateTime.Now;
            // Year of the date time (if date is before current year)
            var formattedYear = date.ToString("yyyy");
            // The Formatted Month string
            var formattedMonth = date.ToString("MMMM");
            // The Formatted time string (only if we are on the same day or the previous day)
            var formattedTime =
                date.DayOfYear == currentDateTime.DayOfYear || date.DayOfYear == currentDateTime.DayOfYear - 1
                    ? string.Format("{0:t}", date)
                    : string.Empty;
            // Check if yesterday
            if (date.DayOfYear == currentDateTime.DayOfYear - 1)
                return useShortName
                    ? "Yesterday"
                    : string.Format("Yesterday - {0}", formattedTime);

            if (date.DayOfYear == currentDateTime.DayOfYear)
                return useShortName
                    ? formattedTime
                    : string.Format("Today - {0}", formattedTime);

            // Return full string by default
            return date.Day + " " + formattedMonth + " " + formattedYear;
        }
    }
}