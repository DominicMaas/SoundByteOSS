using MvvmCross.Converters;
using System;
using System.Globalization;

namespace SoundByte.Core.Converters
{
    /// <summary>
    ///     Takes in a time value, and converts it to a human readable string
    /// </summary>
    public class ReadableDurationConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = new TimeSpan();

            if (value is TimeSpan)
                timeSpan = (TimeSpan)value;

            if (value is int)
                timeSpan = TimeSpan.FromMilliseconds((int)value);

            if (value is double)
                timeSpan = TimeSpan.FromMilliseconds((double)value);

            if (value is string)
                timeSpan = TimeSpan.FromMilliseconds(int.Parse(value.ToString()));

            var returnValue = timeSpan.TotalHours < 1.0
                ? string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds)
                : string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);

            // Return the formatted value
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}