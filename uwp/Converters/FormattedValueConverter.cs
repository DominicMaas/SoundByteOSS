using SoundByte.App.Uwp.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Converters
{
    /// <summary>
    ///     Converts a nullable into to a human readable string
    /// </summary>
    public class FormattedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Get our value
                var inValue = value as double?;

                // Does this null int have a value
                if (inValue.HasValue)
                    return NumberFormatHelper.GetFormattedLargeNumber(inValue.Value);
            }
            catch (Exception)
            {
                return "0";
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}