using Windows.UI.Xaml;
using Plugin.XamJam.Screen.Abstractions;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    ///     UWP Screen Implementation 
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        /// Note: UWP apps do not have access to the maximum screen size, instead this returns the size when the app was launched which is less than ideal: 
        /// http://stackoverflow.com/questions/30335540/get-available-screen-size
        ///     See <see cref="Abstractions.Screen" />
        /// </summary>
        public ScreenSize Size => new ScreenSize(false, Window.Current.Bounds.Width, Window.Current.Bounds.Height);
    }
}