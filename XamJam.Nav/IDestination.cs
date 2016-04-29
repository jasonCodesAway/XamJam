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
        protected PageDestination(TNavScheme navScheme, object viewModel, Page page)
        {
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