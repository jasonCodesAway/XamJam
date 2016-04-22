using System.Windows;
using Plugin.XamJam.Screen.Abstractions;

namespace Plugin.XamJam.Screen
{
    /// <summary>
    ///     Implementation for XamJam.Screen
    /// </summary>
    public class ScreenImplementation : Abstractions.Screen
    {
        /// <summary>
        ///     See <see cref="Abstractions.Screen" />
        /// </summary>
        public ScreenSize Size { get; } = new ScreenSize(Application.Current.RootVisual.RenderSize.Width,
            Application.Current.RootVisual.RenderSize.Height);
    }
}