## View-Model based Navigation Component

#### Disclaimer
This is largely untested and this is intentionally lightweight (aka. feature-poor). I'm happy to accept pull requests for cleanly implemented features.

#### Setup
* Available on [NuGet Xam.Plugins.XamJam.Nav](https://www.nuget.org/packages/Xam.Plugins.XamJam.Nav)

#### Usage
```csharp
// In your PCL's App.xaml.cs, create all your destinations and then navigate to one of them (to setup the App's MainPage)
// The navigator be in charge of knowing how to bring up a Page when asked to bring up it's corresponding ViewModel
var navigator = new Navigator(this);
// The navigation destinations need to reference the navigator so they can navigate on commands
var navigationDestinations = CreateDestinations(navigator);
// And last, the navigator needs to know about all destinations so that it can show the right Page when asked to show a view model
navigator.Initialize(navigationDestinations);

//TODO: Eventually add a regular non-async 'Show' method since the MainPage must be set before this call returns
navigator.ShowAsync<LoginChoicesViewModel>().ConfigureAwait(false);

... 

/**
* Create all your custom views and their corresponding view models and add them to a RootDestination (no back button), a NavDestination (yes back button), or a TabbedDestination (tabs). In this 
* example the first view is a LoginChoicesView which has a LoginChoicesViewModel. You'd need to add *all* of your views here so that ViewModel based navigation works.
*/
private IDestination<INavScheme>[] CreateDestinations(Navigator nav)
{
    var allDestinations= new List<IDestination<INavScheme>>();
    //** Build LoginChoices Destination
    {
        var loginChoicesView = new LoginChoicesView();
        var loginChoicesDestination = new RootDestination<LoginChoicesViewModel>(RootScheme.Singleton,
            new LoginChoicesViewModel(nav, loginChoicesView), new WrapperPage(loginChoicesView));
        allDestinations.Add(loginChoicesDestination);
    }
    return allDestinations.ToArray();
}

```
#### Related projects:
* TODO

#### Related documentation:
* TODO
