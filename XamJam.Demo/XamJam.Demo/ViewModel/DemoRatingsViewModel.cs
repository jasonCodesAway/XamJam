using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Xamarin.Forms;
using XamJam.Nav;
using XamJam.Ratings;

namespace XamJam.Demo.ViewModel
{
    [ImplementPropertyChanged]
    public class DemoRatingsViewModel
    {
        public double Rating { get; set; } = 4.25;

        public int NumStars { get; set; } = 5;

        public Command BackCommand { get; }

        public DemoRatingsViewModel(Navigator nav)
        {
            BackCommand = nav.BackAsyncCommand;
        }
    }
}
