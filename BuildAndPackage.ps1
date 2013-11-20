# Set-ExecutionPolicy RemoteSigned
# valid versions are [2.0, 3.5, 4.0]
$dotNetVersion = "4.0"
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"
$msbuildExe = join-path (Get-ItemProperty $regKey).$regProperty "msbuild.exe"

$solutionName = "CiviKey.WebApi"
$thisDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path )
$ckPackageConsole = join-path $thisDir "\..\Env\Tools\Package\CK.Package.Console.exe"

write "Building Debug..."
& $msbuildExe $thisDir"\$solutionName.sln" /nologo /t:Build /p:DownloadNuGetExe="true" /p:RestorePackages="true" /p:Configuration="Debug" /p:CKPackage="false" /m

write "Building Release..."
& $msbuildExe $thisDir"\$solutionName.sln" /nologo /t:Build /p:DownloadNuGetExe="true" /p:RestorePackages="true" /p:Configuration="Release" /p:CKPackage="false" /m

write "Build CKPackage..."
& $ckPackageConsole $thisDir 0 (join-path $thisDir "Build")