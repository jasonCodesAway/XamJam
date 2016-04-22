using Plugin.XamJam.Screen.Abstractions;
using Android.Content.Res;


namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Implementation for Feature
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
            var width = displayMetrics.HeightPixels / displayMetrics.Density;
            var height = displayMetrics.WidthPixels / displayMetrics.Density;
            Size = new ScreenSize(width, height);
        }
    }
}