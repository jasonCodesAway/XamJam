## ![](phoneSize.png)Xamarin-compatible cross-platform way to determine screen size. 

_Note_: Your application isn't guaranteed to be full screen, so this is the maximum screen size you can hope for, not necessarily the current screen size available to your app.

#### Setup
* Available on [NuGet Xam.Plugins.XamJam.Screen](https://www.nuget.org/packages/Xam.Plugins.XamJam.Screen)
* Install into your PCL project and Client/Platform projects.

#### Usage
```csharp
var size = Plugin.XamJam.Screen.CrossScreen.Current.Size;
var area = size.Width * size.Height;
```
#### Related projects:
* [ACR.deviceinfo](https://github.com/aritchie/deviceinfo) - no support for WP81
* [Xamarin Forms Labs](https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Device)

#### Related documentation:
* [Determining screen size with conditional compilation](http://03cd0a8.netsolhost.com/wordpress/?p=90)
