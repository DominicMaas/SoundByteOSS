using SoundByte.Core.Converters;
using System;
using Xunit;

namespace SoundByte.Core.Tests.Converters
{
    public class ReadableDurationConverterTests : IConverterTest
    {
        [Fact]
        public void TestConvert()
        {
            Assert.Equal("3:15:30", new ReadableDurationConverter().Convert(TimeSpan.FromHours(3) + TimeSpan.FromMinutes(15) + TimeSpan.FromSeconds(30), typeof(string), null, null));
            Assert.Equal("12:00", new ReadableDurationConverter().Convert(TimeSpan.FromMinutes(12), typeof(string), null, null));
            Assert.Equal("0:05", new ReadableDurationConverter().Convert(5000, typeof(string), null, null));
            Assert.Equal("0:15", new ReadableDurationConverter().Convert(15000.3, typeof(string), null, null));
            Assert.Equal("2:30", new ReadableDurationConverter().Convert("150000", typeof(string), null, null));
        }

        [Fact]
        public void TestConvertBack()
        {
            Assert.Throws<NotImplementedException>(() => new ReadableDurationConverter().ConvertBack(50, typeof(string), null, null));
        }
    }
}