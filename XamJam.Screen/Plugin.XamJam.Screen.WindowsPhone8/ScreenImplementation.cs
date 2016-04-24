using System.Windows;
using Plugin.XamJam.Screen.Abstractions;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    ///    WP8 Screen Implementation
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        ///     See <see cref="Abstractions.Screen" />
        /// </summary>
        public ScreenSize Size { get; } = new ScreenSize(true, Application.Current.RootVisual.RenderSize.Width, Application.Current.RootVisual.RenderSize.Height);
    }
}