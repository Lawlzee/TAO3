$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
cd $dir

& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" -nologo -t:Pack TAO3.csproj -p:PackageId=TAO3dev

Remove-Item bin\Feed -Recurse -ErrorAction Ignore
Remove-Item (Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\TAO3dev") -Recurse -ErrorAction Ignore
nuget add bin\Debug\TAO3dev.1.0.0.nupkg -source bin\Feed