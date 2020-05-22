using System;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Converters
{
    /// <summary>
    ///     Converts a bool to either of two strings (passed in as params)
    /// </summary>
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueValue { get; set; }

        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolVal = value as bool?;
            if (!boolVal.HasValue)
                return FalseValue;

            return boolVal.Value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}