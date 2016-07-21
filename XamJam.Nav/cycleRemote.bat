rem must build release first
del Xam.Plugins.XamJam.Nav.*.nupkg
nuget pack Plugin.XamJam.Nav.nuspec
nuget push Xam.Plugins.XamJam.Nav.*.nupkg