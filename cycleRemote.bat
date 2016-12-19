REM grab the project name from the current directory...yay batch files
for %%* in (.) do set rawProjectName=%%~nx*
call set projectNameWithUnderscores=%%rawProjectName:.=_%%%
echo Building %projectNameWithUnderscores%

cd ..
set msbuild.exe=
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild.exe=%%D\MSBuild.exe
if not defined msbuild.exe echo error: can't find MSBuild.exe & goto :eof
if not exist "%msbuild.exe%" echo error: %msbuild.exe%: not found & goto :eof

%msbuild.exe% XamJam.sln /t:%projectNameWithUnderscores% /p:Configuration=Release
cd %rawProjectName%
if %errorlevel% neq 0 exit /b %errorlevel%

del Xam.Plugins.%rawProjectName%.*.nupkg

nuget pack Plugin.%rawProjectName%.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%

nuget push Xam.Plugins.%rawProjectName%.*.nupkg -Source https://www.nuget.org/api/v2/package