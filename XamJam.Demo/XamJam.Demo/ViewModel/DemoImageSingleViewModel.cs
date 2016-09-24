using PropertyChanged;
using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.ViewModel
{
    [ImplementPropertyChanged]
    public class DemoImageSingleViewModel
    {
        public string Text { get; set; } = "Default Single Image View Text";

        public ImageSource ImageSource { get; set; }

        public Command BackCommand { get; }

        public DemoImageSingleViewModel(Navigator navigator)
        {
            BackCommand = navigator.BackAsyncCommand;
        }
    }
}
