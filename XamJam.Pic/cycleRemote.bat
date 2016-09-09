rem must build release first
del Xam.Plugins.XamJam.Pic.*.nupkg
nuget pack Plugin.XamJam.Pic.nuspec -Build -Symbols -Properties Configuration=Release
nuget push Xam.Plugins.XamJam.Pic.*.nupkg -Source https://www.nuget.org/api/v2/package