using System.Drawing;

namespace SoundByte.Core.Helpers
{
    public static class ColorHelper
    {
        public static Color Accent { get; } = Color.FromArgb(255, 198, 138); // ffc68a
        public static Color Background0 { get; } = Color.FromArgb(18, 18, 18); // 121212

        public static Color Background1DP { get; } = Color.FromArgb(30, 30, 30); // 1e1e1e
        public static Color Background4DP { get; } = Color.FromArgb(39, 39, 39); // 272727

        public static Color Text0 { get; } = Color.FromArgb(255, 255, 255);
        public static Color Text1 { get; } = Color.FromArgb(222, 222, 222);
    }
}