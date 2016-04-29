#region

using Xamarin.Forms;

#endregion

namespace XamJam.Nav.Navigation
{
    public class NavigationScheme : INavScheme
    {
        public NavigationScheme(INavScheme parent)
        {
            Parent = parent;
        }

        internal NavigationPage NavigationPage { get; private set; }

        public INavScheme Parent { get; }

        public Page CurrentPage => NavigationPage;

        public SchemeType SchemeType => SchemeType.NavScheme;

        public void SetInitialChild(Page rootPage)
        {
            NavigationPage = new NavigationPage(rootPage);
            NavigationPage.SetHasNavigationBar(NavigationPage, false);
        }
    }
}