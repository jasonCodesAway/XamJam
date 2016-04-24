using Windows.UI.Xaml;
using Plugin.XamJam.Screen.Abstractions;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    /// Windows Phone 8.1 Screen Implementation
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        /// See <see cref="Abstractions.Screen"/>
        /// </summary>
        public ScreenSize Size { get; } = new ScreenSize(true, Window.Current.Bounds.Width, Window.Current.Bounds.Height);
    }
}
