using Xamarin.Forms;

namespace XamJam.Nav.Root
{
    public class RootDestination<TViewModel> : PageDestination<RootScheme>
    {
        public RootDestination(RootScheme navScheme, TViewModel viewModel, View view) : this(navScheme, viewModel, new ContentPage { Content = view })
        {
        }

        public RootDestination(RootScheme navScheme, TViewModel viewModel, Page page) : base(navScheme, viewModel, page)
        {
        }
    }
}