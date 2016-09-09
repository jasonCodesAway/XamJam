using Xamarin.Forms;

namespace XamJam.Pic
{
    public class PicImpl : IPic
    {
        internal PicImpl(Size size, ImageSource imageSource)
        {
            Size = size;
            Source = imageSource;
        }

        public Size Size { get; }

        public ImageSource Source { get; }
    }
}
