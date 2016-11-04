using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamJam.Nav;
using XamJam.Ratings;

namespace XamJam.Demo.ViewModel
{
    public class DemoRatingsViewModel
    {
        public RatingViewModel RatingViewModel { get; } = new RatingViewModel(initialRating:3.5, numStars:5);

        public Command BackCommand { get; }

        public DemoRatingsViewModel(Navigator nav)
        {
            BackCommand = nav.BackAsyncCommand;
        }
    }
}
