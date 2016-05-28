using System;
using Xamarin.Forms;

namespace Plugin.XamJam.Pic.Abstractions
{
  /// <summary>
  /// Interface for XamJam.Pic
  /// </summary>
  public interface IPic
  {

        Size Size { get; }

        Uri Uri { get; }

        ImageSource Source { get; }

        byte[] Bytes { get; }

    }
}
