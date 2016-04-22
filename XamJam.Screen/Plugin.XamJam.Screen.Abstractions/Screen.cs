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
        /// The following references were useful: 
        /// http://03cd0a8.netsolhost.com/wordpress/?p=90
        /// https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Device
        /// https://github.com/aritchie/deviceinfo
        /// </summary>
        ScreenSize Size { get; }
  }
}
