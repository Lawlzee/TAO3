$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
cd $dir

& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" -nologo -t:Pack TAO3.csproj

Remove-Item bin\Feed -Recurse -ErrorAction Ignore
Remove-Item (Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\TAO3") -Recurse -ErrorAction Ignore
nuget add bin\Debug\TAO3.1.0.0.nupkg -source bin\Feed