using Plugin.XamJam.Screen.Abstractions;
using Android.Content.Res;

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
        public ScreenSize Size { get; }

        /// <summary>
        /// Sets the screen's size
        /// </summary>
        public ScreenImplementation()
        {
            var displayMetrics = Resources.System.DisplayMetrics;
            var height = displayMetrics.HeightPixels / displayMetrics.Density;
            var width = displayMetrics.WidthPixels / displayMetrics.Density;
            Size = new ScreenSize(true, width, height);
        }
    }
}