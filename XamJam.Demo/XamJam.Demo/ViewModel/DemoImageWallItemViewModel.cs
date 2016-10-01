using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.ViewModel
{
    [ImplementPropertyChanged]
    public class DemoImageWallItemViewModel
    {
        public Command TappedCommand { get; }

        public ImageSource ImageSource { get; set; }

        public string ImageText { get; set; }

        public DemoImageWallItemViewModel(Navigator navigator)
        {
            TappedCommand = new Command(async () =>
            {
                //show a single image when the user taps on a wall view item
                await navigator.ShowAsync<DemoImageSingleViewModel>(vm =>
                {
                    vm.ImageSource = ImageSource;
                    vm.Text = ImageText;
                });
            });
        }

        public override string ToString()
        {
            return ImageText;
        }
    }
}
