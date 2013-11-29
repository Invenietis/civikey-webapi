task Build-Solution-Debug {
  exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false }
}

task Build-Solution-Release {
  exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Release /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false }
}