using Xamarin.Forms;
using XamJam.Demo.ViewModel;

namespace XamJam.Demo.View
{
    public partial class DemoScreenSizeView : ContentView
    {
        public DemoScreenSizeView()
        {
            InitializeComponent();
            SizeChanged += (sender, args) =>
            {
                // I'm naughty...my View talks to my ViewModel...and I'm 100% okay with that!
                var vm = (DemoScreenSizeViewModel)BindingContext;
                vm.Update();
            };

        }
    }
}
