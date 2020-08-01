using System;
using System.Globalization;
using MvvmCross.Converters;
using SoundByte.Core.Helpers;

namespace SoundByte.Core.Converters
{
    public class DateTimeConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Unknown";

            try
            {
                // Convert to a date time
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
