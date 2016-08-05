rem must build release first
del Xam.Plugins.XamJam.PicSelector.*.nupkg
nuget pack Plugin.XamJam.PicSelector.nuspec
nuget push Xam.Plugins.XamJam.PicSelector.*.nupkg