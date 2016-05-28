using System;
using Plugin.XamJam.Pic.Abstractions;
using Xamarin.Forms;

namespace Plugin.XamJam.Pic
{
    /// <summary>
    /// Generic implementation of IPic so that all devices (iOS, Droid, WP, etc.) can use this single class. 
    /// The platform-dependent logic is relegated to the creation of this class.
    /// </summary>
    public class Pic : IPic
    {
        public Pic(Size size, Uri uri, ImageSource source, byte[] bytes)
        {
            Size = size;
            Uri = uri;
            Source = source;
            Bytes = bytes;
        }

        public Size Size { get; }

        public Uri Uri { get; }

        public ImageSource Source { get; }

        public byte[] Bytes { get; }
    }
}
