namespace Plugin.XamJam.Screen.Abstractions
{
    /// <summary>
    ///     Immutable screen size expressed as a width and height with double precision.
    /// </summary>
    public struct ScreenSize
    {
        /// <summary>
        /// Returns true if this is the maximum screen size your app will encounter on this device. This may be false for UWP apps 
        /// where the screen sizes are not known and there may even be multiple monitors.
        /// </summary>
        public bool IsMaximum { get; }

        /// <summary>
        ///     Screen width
        /// </summary>
        public double Width { get; }

        /// <summary>
        ///     Screen height
        /// </summary>
        public double Height { get; }

        /// <summary>
        ///     Creates a new ScreenSize struct
        /// </summary>
        /// <param name="isMaximum">true if this is a guaranteed maximum screen size, false for cases where the maximum screen size is not known (e.g. UWP with multiple monitors)</param>
        /// <param name="width">the screen's width</param>
        /// <param name="height">the screen's height</param>
        public ScreenSize(bool isMaximum, double width, double height)
        {
            IsMaximum = isMaximum;
            Width = width;
            Height = height;
        }
    }
}