using Xamarin.Forms;
using XamJam.Nav;

namespace XamJam.Demo.View
{
    /// <summary>
    /// This is ugly. 
    /// Does anyone know how to expose something like the below WallItemView and WallView so they work in XAML only?
    /// In WPF this is done with templates, but there is this quirckyness here with the interplay between the dynamic
    /// current screen real-estate and the individual's image size and paging. 
    /// </summary>
    public partial class DemoImageWallView : ContentView
    {
        public DemoImageWallView(Navigator navigator)
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}
