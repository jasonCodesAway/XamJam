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
        public double Width { get; }

        public double Height { get; }

        public ScreenSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
