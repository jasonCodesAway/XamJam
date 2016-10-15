namespace Plugin.XamJam.Screen.Abstractions
{
    /// <summary>
    ///     Interface for XamJam.Screen
    /// </summary>
    public interface Screen
    {
        /// <summary>
        /// Returns this platforms current total screen size. Only UWP has dynamic screen sizes (e.g. plugging in a new monitor, changing resolutions, etc.)
        /// </summary>
        ScreenSize Size { get; }
    }
}