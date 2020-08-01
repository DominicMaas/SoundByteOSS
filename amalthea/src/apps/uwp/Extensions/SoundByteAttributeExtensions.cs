using SoundByte.App.UWP.Presenters;
using System;
using System.Linq;

namespace SoundByte.App.UWP.Extensions
{
    public static class SoundByteAttributeExtensions
    {
        public static bool HasTabAttribute(this Type view)
        {
            var attributes = view
                .GetCustomAttributes(typeof(SoundByteTabAttribute), true);

            return attributes.Any();
        }

        public static SoundByteTabAttribute GetTabAttribute(this Type view)
        {
            var attributes = view
                .GetCustomAttributes(typeof(SoundByteTabAttribute), true);

            return (SoundByteTabAttribute)attributes.FirstOrDefault();
        }

        public static bool HasModalAttribute(this Type view)
        {
            var attributes = view
                .GetCustomAttributes(typeof(SoundByteModalAttribute), true);

            return attributes.Any();
        }

        public static SoundByteModalAttribute GetModalAttribute(this Type view)
        {
            var attributes = view
                .GetCustomAttributes(typeof(SoundByteModalAttribute), true);

            return (SoundByteModalAttribute)attributes.FirstOrDefault();
        }
    }
}