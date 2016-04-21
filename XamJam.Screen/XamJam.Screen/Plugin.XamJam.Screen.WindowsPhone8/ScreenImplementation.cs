using Plugin.XamJam.Screen.Abstractions;
using System;
using System.Windows;


namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Implementation for XamJam.Screen
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        public ScreenSize Size { get; } = new ScreenSize(Application.Current.RootVisual.RenderSize.Width, Application.Current.RootVisual.RenderSize.Height);
    }
}