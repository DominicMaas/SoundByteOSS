using MvvmCross.Converters;
using System;
using System.Globalization;
using UIKit;

namespace SoundByte.App.iOS.Converters
{
    public class UIImageConverter : MvxValueConverter<string, UIImage>
    {
        protected override UIImage Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return new UIImage(value);
        }
    }
}