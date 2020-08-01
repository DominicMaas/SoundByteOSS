﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SoundByte.App.UWP.Converters
{
    /// <summary>
    ///     Allows converting bools to solid color brushes
    /// </summary>
    public class BooleanToBrushConverter : IValueConverter
    {
        public string TrueColor { get; set; }

        public string FalseColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolVal = value as bool?;

            if (!boolVal.HasValue)
                return Application.Current.Resources[FalseColor] as Brush;

            var returnVal = Application.Current.Resources[boolVal.Value ? TrueColor : FalseColor] as Brush;

            return returnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
