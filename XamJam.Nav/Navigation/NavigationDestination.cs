using Xamarin.Forms;

namespace XamJam.Nav.Navigation
{
    public class NavigationDestination<TViewModel> : PageDestination<NavigationScheme>
    {
        public NavigationDestination(NavigationScheme navScheme, TViewModel viewModel, Page page)
            : base(navScheme, viewModel, page)
        {
        }
    }
}