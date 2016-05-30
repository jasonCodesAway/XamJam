using System;
using Xamarin.Forms;

namespace Plugin.XamJam.Pic.Abstractions
{
  /// <summary>
  /// Interface for XamJam.Pic
  /// </summary>
  public interface IPic
  {

        /// <summary>
        /// 
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// 
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// 
        /// </summary>
        ImageSource Source { get; }

        /// <summary>
        /// 
        /// </summary>
        byte[] Bytes { get; }

    }
}
