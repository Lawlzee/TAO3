$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
cd $dir

dotnet build --nologo -v q --configuration Release
dotnet pack TAO3.csproj /p:PackageVersion=1.0.0 --nologo -v q

Remove-Item bin\Feed -Recurse -ErrorAction Ignore
Remove-Item (Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\TAO3") -Recurse -ErrorAction Ignore
nuget add bin\Debug\TAO3.1.0.0.nupkg -source bin\Feed