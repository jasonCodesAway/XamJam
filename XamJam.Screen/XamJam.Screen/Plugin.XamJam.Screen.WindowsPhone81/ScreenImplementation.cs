using Plugin.XamJam.Screen.Abstractions;
using System;
using Windows.UI.Xaml;


namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Implementation for XamJam.Screen
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        public ScreenSize Size { get; } = new ScreenSize(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
    }
}