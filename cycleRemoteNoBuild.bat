REM grab the project name from the current directory...yay batch files
for %%* in (.) do set rawProjectName=%%~nx*
call set projectNameWithUnderscores=%%rawProjectName:.=_%%%

cd %rawProjectName%
if %errorlevel% neq 0 exit /b %errorlevel%

del Xam.Plugins.%rawProjectName%.*.nupkg

nuget pack Plugin.%rawProjectName%.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%

nuget push Xam.Plugins.%rawProjectName%.*.nupkg -Source https://www.nuget.org/api/v2/package