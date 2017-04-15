using Plugin.XamJam.Screen;
using PropertyChanged;
using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.ViewModel
{
    [ImplementPropertyChanged]
    public class DemoScreenSizeViewModel
    {
        public Command BackCommand { get; }

        public double Width { get; set; }

        public double Height { get; set; }

        public DemoScreenSizeViewModel(Navigator navigator)
        {
            BackCommand = navigator.BackAsyncCommand;
        }

        internal void Update()
        {
            var cWidth = CrossScreen.Current.Size.Width;
            var cHeight = CrossScreen.Current.Size.Height;
            if (Width != cWidth)
            {
                Width = cWidth;
            }
            if (Height != cHeight)
            {
                Height = cHeight;
            }
        }
    }
}
