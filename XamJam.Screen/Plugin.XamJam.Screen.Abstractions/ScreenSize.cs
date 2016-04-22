using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.XamJam.Screen.Abstractions
{
    /// <summary>
    /// Immutable screen size expressed as a width and height with double precision.
    /// </summary>
    public struct ScreenSize
    {
        /// <summary>
        /// Screen width
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Screen height
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Creates a new ScreenSize struct
        /// </summary>
        /// <param name="width">the screen's width</param>
        /// <param name="height">the screen's height</param>
        public ScreenSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
