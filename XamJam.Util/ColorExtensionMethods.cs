using System.Globalization;
using Xamarin.Forms;

namespace XamJam.Util
{
    public static class ColorExtensionMethods
    {
        public static string ToPlainHex(this Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static Color FromPlainHex(this string plainHex)
        {
            var argb = uint.Parse(plainHex, NumberStyles.HexNumber);
            return Color.FromUint(argb);
        }
    }
}