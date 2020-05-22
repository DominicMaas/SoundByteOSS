using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Converters
{
    public class BoolVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Invert)
            {
                if (value is bool booleanValue)
                    return booleanValue ? Visibility.Collapsed : Visibility.Visible;
                return Visibility.Visible;
            }
            else
            {
                if (value is bool booleanValue)
                    return booleanValue ? Visibility.Visible : Visibility.Collapsed;
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}