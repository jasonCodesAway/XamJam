using Xamarin.Forms;

namespace XamJam.Nav
{
    public interface INavScheme
    {
        SchemeType SchemeType { get; }

        INavScheme Parent { get; }

        Page CurrentPage { get; }

        bool IsDisplayed { get; set; }
    }
}