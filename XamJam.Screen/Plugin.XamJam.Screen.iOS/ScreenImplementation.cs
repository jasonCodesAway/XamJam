using Plugin.XamJam.Screen.Abstractions;
using UIKit;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// iOS (iPad + iPhone) Screen Implementation
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        /// See <see cref="Abstractions.Screen"/>
        /// </summary>
        public ScreenSize Size { get; } = new ScreenSize(true, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
    }
}