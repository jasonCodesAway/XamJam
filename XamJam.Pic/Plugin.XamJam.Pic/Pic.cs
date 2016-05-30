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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="uri"></param>
        /// <param name="source"></param>
        /// <param name="bytes"></param>
        public Pic(Size size, Uri uri, ImageSource source, byte[] bytes)
        {
            Size = size;
            Uri = uri;
            Source = source;
            Bytes = bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// 
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// 
        /// </summary>
        public ImageSource Source { get; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Bytes { get; }
    }
}
