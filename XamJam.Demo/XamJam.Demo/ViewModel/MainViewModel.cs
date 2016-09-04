using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
