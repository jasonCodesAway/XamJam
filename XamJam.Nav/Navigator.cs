#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;
using XamJam.Nav.Navigation;
using XamJam.Nav.Root;
using XamJam.Nav.Tab;

#endregion

namespace XamJam.Nav
{
    public class Navigator
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(Navigator));

        private readonly Application application;

        private readonly IDictionary<object, IDestination<INavScheme>> vm2Destinations =
            new Dictionary<object, IDestination<INavScheme>>();

        private readonly IDictionary<Type, IDestination<INavScheme>> vmType2Destinations =
            new Dictionary<Type, IDestination<INavScheme>>();

        public Navigator(Application application)
        {
            this.application = application;
            BackAsyncCommand = new Command(BackAsync);
            NavigateToDestinationAsyncCommand = new Command<IDestination<INavScheme>>(async dest => await ShowAsync(dest.ViewModel));
        }

        public Command BackAsyncCommand { get; }

        public Command<IDestination<INavScheme>> NavigateToDestinationAsyncCommand { get; }

        public Command CreateNavigationCommand<TVm>(Action<TVm> setupViewModel = null)
        {
            return new Command(async () => await ShowAsync(setupViewModel));
        }

        public void Initialize(params IDestination<INavScheme>[] destinations)
        {
            if (vmType2Destinations.Count != 0)
                Monitor.Throw("Duplicate call to #Initialize");

            foreach (var destination in destinations)
            {
                vm2Destinations[destination.ViewModel] = destination;
                vmType2Destinations[destination.ViewModel.GetType()] = destination;
            }
        }

        public void Show<TVm>(Action<TVm> setupState = null)
        {
            ShowAsync(setupState).ConfigureAwait(false);
        }

        public async Task ShowAsync<TVm>(Action<TVm> setupState = null)
        {
            var hasDestination = vmType2Destinations.ContainsKey(typeof (TVm));
            if (!hasDestination)
                Monitor.Throw($"Found no destination for view model type: {typeof(TVm)}");
            var destination = vmType2Destinations[typeof(TVm)];
            await ShowAsync(destination, setupState);
        }

        public async Task ShowAsync<TVm>(TVm destinationViewModel, Action<TVm> setupState = null)
        {
            var hasDestination = vmType2Destinations.ContainsKey(typeof(TVm));
            if (!hasDestination)
                Monitor.Throw($"Found no destination for view model type: {typeof(TVm)}");
            var destination = vm2Destinations[destinationViewModel];
            await ShowAsync(destination, setupState);
        }

        private IDestination<INavScheme> currentDestination;

        public async Task ShowAsync<TVm>(IDestination<INavScheme> destination, Action<TVm> setupState = null)
        {
            Monitor.Debug($"Showing Destination {destination} for view model {typeof(TVm)} with setup: {setupState}");

            // Setup the ViewModel
            setupState?.Invoke((TVm)destination.ViewModel);

            // Display the new page. 
            switch (destination.NavScheme.SchemeType)
            {
                // Make sure the right tab is displayed, then make sure the parent of the tab is displayed
                case SchemeType.TabScheme:
                    var tabDestination = (TabDestination)destination;
                    // Make sure the right tab is displayed
                    tabDestination.NavScheme.Show(tabDestination);
                    // Make sure the parent of the tab view is displayed
                    //TODO: FIXME: WTH!
                    //if (!tabDestination.IsDisplayed)
                    //{
                    //}
                    if (!tabDestination.View.IsVisible)
                    {
                        Debug.WriteLine("Okay, now show this sucker");
                    }
                    // If the tab page isn't the main page
                    //if (application.MainPage != tabDestination.NavScheme.TabbedPage)
                    //{
                    //    // if the main page is a nav page, then push the tab page
                    //    if (currentDestination.NavScheme.SchemeType == SchemeType.NavScheme)
                    //    {
                    //        Monitor.Debug("Pushing tabbed page on top of Navigation Page");
                    //        await currentDestination.NavScheme.CurrentPage.Navigation.PushAsync(tabDestination.NavScheme.TabbedPage);
                    //    }
                    //    // else set the current page to the tabbed page
                    //    else if (tabDestination.NavScheme.Parent?.CurrentPage != null && 
                    //        tabDestination.NavScheme.Parent.SchemeType == SchemeType.NavScheme)
                    //    {
                    //        Monitor.Debug($"Setting tab's navigation parent: {tabDestination.NavScheme.Parent.CurrentPage.GetType().Name} as the root page");
                    //        application.MainPage = tabDestination.NavScheme.Parent.CurrentPage;
                    //        //do we have to push the tab on the parent here?
                    //        //await tabDestination.NavScheme.Parent.CurrentPage.Navigation.PushAsync(tabDestination.NavScheme.TabbedPage);
                    //    }
                    //    else
                    //    {
                    //        Monitor.Debug("Setting tabbed page as the root page");
                    //        application.MainPage = tabDestination.NavScheme.TabbedPage;
                    //    }
                    //}
                    break;
                case SchemeType.NavScheme:
                    var navDestination = (NavigationDestination<TVm>)destination;
                    Monitor.Debug($"Pushing {navDestination.Page} on NavigationPage: {navDestination.NavScheme.NavigationPage} App.MainPage: {application.MainPage}");
                    await navDestination.PushAsync();
                    // If the nav page isn't showing, then implicitly make it the main page
                    if (!navDestination.NavScheme.NavigationPage.IsVisible)
                        application.MainPage = navDestination.NavScheme.NavigationPage;
                    break;
                case SchemeType.Root:
                    var rootDestination = (RootDestination<TVm>)destination;
                    rootDestination.NavScheme.CurrentPage = rootDestination.Page;
                    application.MainPage = rootDestination.Page;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentDestination = destination;
        }

        private async void BackAsync()
        {
            await application.MainPage.Navigation.PopAsync();
        }
    }
}