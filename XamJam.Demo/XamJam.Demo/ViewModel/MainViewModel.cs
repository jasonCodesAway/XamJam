using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.ViewModel
{
    public class MainViewModel
    {
        public Command ShowImageWallCommand { get; }

        public MainViewModel(Navigator nav)
        {
            ShowImageWallCommand = nav.CreateNavigationCommand<DemoImageWallViewModel>();
        }
    }
}
