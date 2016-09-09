rem must build release first
del Xam.Plugins.XamJam.PicSelector.*.nupkg
nuget pack Plugin.XamJam.PicSelector.nuspec -Build -Symbols -Properties Configuration=Release
nuget push Xam.Plugins.XamJam.PicSelector.*.nupkg -Source https://www.nuget.org/api/v2/package