rem must build release first
del Xam.Plugins.XamJam.Pic.*.nupkg
nuget pack Plugin.XamJam.Pic.nuspec
nuget push Xam.Plugins.XamJam.Pic.*.nupkg