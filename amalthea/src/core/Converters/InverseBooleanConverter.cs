using MvvmCross.Converters;
using System;
using System.Globalization;

namespace SoundByte.Core.Converters
{
    public class InverseBooleanConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (bool)value;
            return !input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (bool)value;
            return !input;
        }
    }
}