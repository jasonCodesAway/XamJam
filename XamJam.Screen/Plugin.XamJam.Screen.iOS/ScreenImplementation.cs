using Plugin.XamJam.Screen.Abstractions;
using System;
using UIKit;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Implementation for XamJam.Screen
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        /// See <see cref="Abstractions.Screen"/>
        /// </summary>
        public ScreenSize Size { get; } = new ScreenSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
    }
}