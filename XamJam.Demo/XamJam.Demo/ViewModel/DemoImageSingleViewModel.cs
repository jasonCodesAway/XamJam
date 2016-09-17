using PropertyChanged;
using Xamarin.Forms;

namespace XamJam.Demo.ViewModel
{
    [ImplementPropertyChanged]
    public class DemoImageSingleViewModel
    {
        public string Text { get; set; } = "Default Text";

        public ImageSource ImageSource { get; set; }
    }
}
