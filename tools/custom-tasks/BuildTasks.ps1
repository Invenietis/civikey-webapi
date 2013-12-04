Import-Module .\tools\custom-modules\SharedAssemblyInfoEditor.ps1

task Build-Solution-Debug {
    Update-SharedAssemblyInfo
    
    try {
        exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:MvcBuildViews=false /p:CKPackage=false }
    }
    finally {
        Reset-SharedAssemblyInfo
    }
}

task Build-Solution-Release {
    Update-SharedAssemblyInfo

    try {
        exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Release /p:DownloadNuGetExe=false /p:RestorePackages=false /p:MvcBuildViews=true /p:CKPackage=false }
    }
    finally {
        Reset-SharedAssemblyInfo
    }
}