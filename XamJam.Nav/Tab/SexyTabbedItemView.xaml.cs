using Xamarin.Forms;

namespace XamJam.Nav.Tab
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SexyTabbedItemView : ContentView
    {
        public SexyTabbedItemView(TabDestination viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
