## ![](phoneSize.png) Xamarin-compatible cross-platform screen size getter. 

#### Disclaimer
Your application isn't guaranteed to be full screen (e.g. Android allows multiple apps to simultaneously share the screen now), so this is the maximum screen size you can hope for, not necessarily the current screen size available to your app. *And* [UWP doesn't provide the maximum screen size](http://stackoverflow.com/questions/30335540/get-available-screen-size) (since it can change resolutions, potentially has multiple monitors, etc.) so for UWP you only get the initial screen size of the application and you have to check the [ScreenSize.IsMaximum method](https://github.com/jasonCodesAway/XamJam/blob/master/XamJam.Screen/Plugin.XamJam.Screen.Abstractions/ScreenSize.cs) to determine if the screen size really is guaranteed to be the maximum screen size your app will ever be displayed with. Consequently this plugin is of limited use (I only use it to pre-create enough special image views to fill the screen). 

#### Setup
* Available on [NuGet Xam.Plugins.XamJam.Screen](https://www.nuget.org/packages/Xam.Plugins.XamJam.Screen)
* Install into your PCL project and Client/Platform projects.

#### Usage
```csharp
var size = Plugin.XamJam.Screen.CrossScreen.Current.Size;
var maxArea = size.Width * size.Height;
var isGuaranteedMax = size.IsMaximum;
```
#### Related projects:
* [ACR.deviceinfo](https://github.com/aritchie/deviceinfo) - no support for WP81
* [Xamarin Forms Labs](https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Device)

#### Related documentation:
* [Determining screen size with conditional compilation](http://03cd0a8.netsolhost.com/wordpress/?p=90)
