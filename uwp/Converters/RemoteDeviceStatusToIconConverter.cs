using System;
using Windows.UI.Xaml.Data;

namespace SoundByte.App.Uwp.Converters
{
    public class RemoteDeviceStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var kind = value?.ToString().ToLower();

            switch (kind)
            {
                case "xbox":
                    return "\uE990";

                case "desktop":
                    return "\uE977";

                case "laptop":
                    return "\uE7F8";

                case "phone":
                    return "\uE8EA";

                case "tablet":
                    return "\uE70A";

                default:
                    return "\uF142";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}