using SoundByte.App.Uwp.Helpers;
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Converters
{
    /// <summary>
    ///     This class takes in a DateTime object and converts it into
    ///     a human readable form.
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        /// <summary>
        ///     This function takes in a datetime string and converts it
        ///     into a friendly readable string for the UI.
        /// </summary>
        /// <returns>A human readable date time object</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "Unknown";

            try
            {
                // Convert to a datetime
                var inputDate = DateTime.Parse(value.ToString());

                // Return the formatted DateTime
                return NumberFormatHelper.GetTimeDateString(inputDate, true);
            }
            catch (Exception)
            {
                // There was an error either parsing the value or converting the
                // date time. We will show a generic unknown message here.
                return "Unknown";
            }
        }

        /// <summary>
        ///     This function is not needed and should not be used.
        ///     It returns the current date time just in case it is
        ///     called.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }
    }
}