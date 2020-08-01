using SoundByte.Core.Converters;
using Xunit;

namespace SoundByte.Core.Tests.Converters
{
    public class InverseBooleanConverterTests : IConverterTest
    {
        [Fact]
        public void TestConvert()
        {
            Assert.False((bool)new InverseBooleanConverter().Convert(true, typeof(bool), null, null));
            Assert.True((bool)new InverseBooleanConverter().Convert(false, typeof(bool), null, null));
        }

        [Fact]
        public void TestConvertBack()
        {
            Assert.False((bool)new InverseBooleanConverter().ConvertBack(true, typeof(bool), null, null));
            Assert.True((bool)new InverseBooleanConverter().ConvertBack(false, typeof(bool), null, null));
        }
    }
}