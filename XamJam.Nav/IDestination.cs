#region

using Xamarin.Forms;

#endregion

namespace XamJam.Nav
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TNavScheme"></typeparam>
    public interface IDestination<out TNavScheme> where TNavScheme : INavScheme
    {
        object ViewModel { get; }

        TNavScheme NavScheme { get; }
    }

    /// <summary>
    ///     Convenience base class for any destinations that are visually represented by a simple Page
    /// </summary>
    /// <typeparam name="TNavScheme"></typeparam>
    public abstract class PageDestination<TNavScheme> : IDestination<TNavScheme> where TNavScheme : INavScheme
    {
        private static Page Wrap(View view)
        {
            var page = new ContentPage
            {
                Content = view
            };
            // Don't let the Page draw over the iPhone & iPad's status bar
            Device.OnPlatform(() =>
            {
                page.Padding = new Thickness(0, 20, 0, 0);
            });
            return page;
        }

        protected PageDestination(TNavScheme navScheme, object viewModel, View view) : this(navScheme, viewModel, Wrap(view))
        {
        }

        protected PageDestination(TNavScheme navScheme, object viewModel, Page page)
        {
            NavigationPage.SetHasNavigationBar(page, false);
            NavScheme = navScheme;
            ViewModel = viewModel;
            Page = page;
            page.BindingContext = viewModel;
        }

        public Page Page { get; }

        public object ViewModel { get; }

        public TNavScheme NavScheme { get; }

        public override string ToString()
        {
            return $"PageDestination, NavScheme={NavScheme.SchemeType}, ViewModel={ViewModel.GetType().Name}, Page={Page.GetType()}";
        }
    }
}