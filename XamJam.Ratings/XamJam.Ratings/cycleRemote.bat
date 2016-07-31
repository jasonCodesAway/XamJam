rem must build release first
del Xam.Plugins.XamJam.Ratings.*.nupkg
nuget pack Plugin.XamJam.Ratings.nuspec
nuget push Xam.Plugins.XamJam.Ratings.*.nupkg