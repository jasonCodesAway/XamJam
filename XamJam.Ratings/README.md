## A 5-star rating component with 1/2 star increments

#### Disclaimer
This is largely untested and this is intentionally lightweight (aka. feature-poor). I'm happy to accept pull requests for cleanly implemented features.

#### Setup
* Available on [NuGet Xam.Plugins.XamJam.Ratings](https://www.nuget.org/packages/Xam.Plugins.XamJam.Ratings)

#### Usage
```csharp
// Expose the RatingViewModel instance in your own ViewModel
public RatingViewModel Rating { get; } = new RatingViewModel();

// And in XAML:
<ratings:RatingView BindingContext="{Binding Rating}" HorizontalOptions="CenterAndExpand" VerticalOptions="StartAndExpand"/>
```
#### Related projects:
* TODO

#### Related documentation:
* TODO
