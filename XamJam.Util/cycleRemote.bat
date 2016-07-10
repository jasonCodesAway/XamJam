rem must build release first
del Xam.Plugins.XamJam.Util.*.nupkg
nuget pack Plugin.XamJam.Util.nuspec
nuget push Xam.Plugins.XamJam.Util.*.nupkg