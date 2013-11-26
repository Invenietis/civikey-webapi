Import-Module .\tools\CK-Package4PS\CK.Package.Components.psd1

framework "4.0"

properties {
    $solution = @{}
    $solution.Directory = "src"
    $solution.FileName = "CiviKey.WebApi"
    $solution.TestProjects = Get-ChildItem -Path $solution.Directory -File -Filter "*.Tests.csproj" -Recurse
    $solution.TestDllsFormat = "*.Tests.dll"
    $solution.TestFramework = "net-4.0"

    $output = @{}
    $output.TestsDirectory = ".\output\tests"
    $output.PackageDirectory = ".\output\package"

    $packages = @{}
    $packages.PackagesDirectory = Join-Path $solution.Directory "packages"
    $packages.ConfigFiles = Get-ChildItem -Path $solution.Directory -Filter "packages.config" -Recurse

    $ckpackage = @{}
    $ckpackage.SolutionDirectory = ".\"
    $ckpackage.OutputDirectory = ".\output\package"
}

#region Utility functions and tasks

function Get-SolutionPath {
    return (Get-Item (Join-Path $solution.Directory ("{0}.sln" -f $solution.FileName))).FullName
}

task Initialize-Directories {   
    if( Test-Path $output.TestsDirectory ) {
        Remove-Item -Path $output.TestsDirectory -Force -Recurse
    }
    if( Test-Path $output.PackageDirectory ) {
        Remove-Item -Path $output.PackageDirectory -Force -Recurse
    }
    
    $output.TestsDirectory = (New-Item -Force -ItemType Directory $output.TestsDirectory).FullName
    $output.PackageDirectory = (New-Item -Force -ItemType Directory $output.PackageDirectory).FullName
}

task NugetRestore {
    $packages.configFiles | % {
        $packagesDir = $packages.PackagesDirectory
        exec { nuget restore $_.FullName -PackagesDirectory $packagesDir }
    }
}

#endregion

#region Build tasks

task Build-Solution-Debug {
  exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false }
}

task Build-Solution-Release {
  exec { msbuild (Get-SolutionPath) /nologo /p:Configuration=Release /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false }
}

task Build -depends Initialize-Directories, NugetRestore, Build-Solution-Debug, Build-Solution-Release

#endregion

#region Unit tests tasks

task Download-NUnitConsole {
    $packagesDir = $output.TestsDirectory
    exec { nuget install NUnit.Runners -OutputDirectory $packagesDir }
}

task Build-UnitTests {
  $solution.TestProjects | % {
    $td = $output.TestsDirectory
    exec { msbuild $_.FullName /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false /p:OutputPath=$td }
  }
}

task Run-NUnitTests {
    $nunitConsole = Get-ChildItem -Path $output.TestsDirectory -File -Filter nunit-console.exe -Recurse
    $dlls = (Get-ChildItem -Path $output.TestsDirectory -File $solution.TestDllsFormat).FullName
    $testFramework = $solution.TestFramework

    exec { & $nunitConsole.FullName $dlls /framework:$testFramework }
}

task UnitTest -depends Initialize-Directories, Download-NUnitConsole, NugetRestore, Build-UnitTests, Run-NUnitTests

#endregion

#region CKPackage

task Build-CKPackage {
    exec { Invoke-CKPackage -SolutionDirectory (get-item $ckpackage.SolutionDirectory) -OutputDirectory (get-item $ckpackage.OutputDirectory) }
}

task CKPackage -depends Initialize-Directories, Build-CKPackage

#endregion

task Default -depends Build, UnitTest, CKPackage