﻿function Get-SolutionPath {
    return (Get-Item (Join-Path $solution.Directory ("{0}.sln" -f $solution.FileName))).FullName
}

function Get-CurrentVersion() {
    $ckSolutionFile = Get-Item (Join-Path $ckpackage.SolutionDirectory "Solution.ck")
    $versionTag  = (Get-Content -Encoding UTF8 $ckSolutionFile | ? { $_.Trim().StartsWith( "<Version" ) }).Trim()

    $matches = [regex]::Match($versionTag, "<Version Major=(.+) Minor=(.+) Patch=(.+) />")

    return "{0}.{1}.{2}" -f $matches.Groups[1].Value.Replace('"', [string]::Empty), $matches.Groups[2].Value.Replace('"', [string]::Empty), $matches.Groups[3].Value.Replace('"', [string]::Empty)
}

task Initialize-Directories { 
    if( Test-Path $outputDirectory ) {
        Remove-Item -Path $outputDirectory -Force -Recurse
    }
    
    $script:outputDirectory = (New-Item -Force -ItemType Directory $outputDirectory).FullName
}

task NugetRestore {
    exec { 
        $packages.configFiles | % {
            $packagesDir = $packages.PackagesDirectory
            nuget restore $_.FullName -PackagesDirectory $packagesDir
        }
    }
}