#region

using System;
using System.Collections.Generic;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;

#endregion

namespace XamJam.Nav.Tab
{
    /// <summary>
    ///     A collection of tabs where you can show one at a time
    /// </summary>
    public class TabScheme : INavScheme
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(TabScheme));

        private TabDestination currentlyVisible;

        private SexyTabbedView view;

        public TabScheme(INavScheme parent)
        {
            Parent = parent;
        }

        public bool IsTabOnTop { get; set; } = Device.OS != TargetPlatform.iOS;

        public IEnumerable<TabDestination> Children { get; private set; }

        public Page TabbedPage { get; private set; }

        public SchemeType SchemeType => SchemeType.TabScheme;

        public INavScheme Parent { get; }

        public Page CurrentPage => TabbedPage;

        public void SetChildren(Func<View, ContentPage> pageCreator = null, params TabDestination[] children)
        {
            if (pageCreator == null)
                pageCreator = v => new ContentPage {Content = v};

            Children = children;
            // every time a child is clicked on, show it's content
            foreach (var child in children)
            {
                child.ClickedCommand = new Command(() => Show(child));
            }
            view = new SexyTabbedView(IsTabOnTop, children);
            var page = pageCreator(view);
            TabbedPage = page;
            NavigationPage.SetHasNavigationBar(TabbedPage, false);
            view.BindingContext = this;
        }


        public void Show(TabDestination showMe)
        {
            if (currentlyVisible != null)
            {
                currentlyVisible.IsSelected = false;
            }
            currentlyVisible = showMe;
            Monitor.Debug($"Showing: {showMe.View.GetType()}");
            view.Show(showMe);
            currentlyVisible.IsSelected = true;
        }
    }
}