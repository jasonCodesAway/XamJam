using Android.Content.Res;
using Plugin.XamJam.Screen.Abstractions;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Adnroid Screen Implementation
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        /// See <see cref="Abstractions.Screen"/>
        /// </summary>
        public ScreenSize Size
        {
            get
            {
                var displayMetrics = Resources.System.DisplayMetrics;
                var height = displayMetrics.HeightPixels / displayMetrics.Density;
                var width = displayMetrics.WidthPixels / displayMetrics.Density;
                return new ScreenSize(true, width, height);
            }
        }
    }
}