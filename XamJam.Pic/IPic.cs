using Xamarin.Forms;

namespace XamJam.Pic
{
    public interface IPic
    {
        Size Size { get; }

        ImageSource Source { get; }
    }
}
