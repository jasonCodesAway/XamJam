using System;
using System.Drawing;

namespace Plugin.XamJam.Screen.Abstractions
{
  /// <summary>
  /// Interface for XamJam.Screen
  /// </summary>
  public interface Screen
  {
        /// <summary>
        /// Returns this platforms total screen size. The size available to any app at any point in time will not exceed this size.
        /// </summary>
        ScreenSize Size { get; }
  }
}
