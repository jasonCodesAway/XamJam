using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.ViewModel
{
    public class MainViewModel
    {
        public Command ShowImageWall { get; }

        public MainViewModel(Navigator nav)
        {
            ShowImageWall = nav.CreateNavigationCommand<DemoImageWallViewModel>();
        }
    }
}
