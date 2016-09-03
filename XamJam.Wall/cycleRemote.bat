rem must build release first
del Xam.Plugins.XamJam.Wall.*.nupkg
nuget pack Plugin.XamJam.Wall.nuspec
nuget push Xam.Plugins.XamJam.Wall.*.nupkg