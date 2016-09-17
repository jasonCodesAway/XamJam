using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamJam.Nav.Navigation
{
    public class NavigationDestination<TViewModel> : PageDestination<NavigationScheme>
    {
        public NavigationDestination(NavigationScheme navScheme, TViewModel viewModel, View view) : base(navScheme, viewModel, view)
        {
        }

        public NavigationDestination(NavigationScheme navScheme, TViewModel viewModel, Page page) : base(navScheme, viewModel, page)
        {
        }

        public async Task PushAsync()
        {
            if (NavScheme.NavigationPage == null)
                NavScheme.SetupNavigationPage(Page);
            await NavScheme.NavigationPage.PushAsync(Page, false);
        }
    }
}