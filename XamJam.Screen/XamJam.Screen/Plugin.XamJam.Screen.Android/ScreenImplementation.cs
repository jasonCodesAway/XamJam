using Plugin.XamJam.Screen.Abstractions;
using System;
using Android.Content.Res;


namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        public ScreenSize Size { get; }

        public ScreenImplementation()
        {
            var displayMetrics = Resources.System.DisplayMetrics;
            var width = displayMetrics.HeightPixels / displayMetrics.Density;
            var height = displayMetrics.WidthPixels / displayMetrics.Density;
            Size = new ScreenSize(width, height);
        }
    }
}