using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     Methods for working with strings and converting objects to strings
    /// </summary>
    public static class StringHelper
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
                    : string.Format("{0} - {1}", "Yesterday", formattedTime);

            if (date.DayOfYear == currentDateTime.DayOfYear)
                return useShortName
                    ? formattedTime
                    : string.Format("{0} - {1}", "Today", formattedTime);

            // Return full string by default
            return date.Day + " " + formattedMonth + " " + formattedYear;
        }

        /// <summary>
        ///     Calculate a MD5 hash from the string
        /// </summary>
        /// <param name="input">The text to convert to a MD5 hash</param>
        /// <returns>A Md5 hash</returns>
        public static string CalculateMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                // Calculate MD5 hash from input
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);

                // Convert byte array to hex string
                var sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            var buffer = Encoding.Unicode.GetBytes(text);
            var ms = new MemoryStream();

            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;

            var compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            var gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        /// <summary>
        ///     Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            var gzBuffer = Convert.FromBase64String(compressedText);
            using (var ms = new MemoryStream())
            {
                var msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                var buffer = new byte[msgLength];

                ms.Position = 0;
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
            }
        }
    }
}