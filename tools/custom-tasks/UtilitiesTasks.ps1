function Get-SolutionPath {
    return (Get-Item (Join-Path $solution.Directory ("{0}.sln" -f $solution.FileName))).FullName
}

function Get-CurrentVersion() {
    $ckSolutionFile = Get-Item (Join-Path $ckpackage.SolutionDirectory "Solution.ck")
    $versionTag  = (Get-Content -Encoding UTF8 $ckSolutionFile | ? { $_.Trim().StartsWith( "<Version" ) }).Trim()

    $matches = [regex]::Match($versionTag, "<Version Major=(.+) Minor=(.+) Patch=(.+) />")

    return "{0}.{1}.{2}" -f $matches.Groups[1].Value.Replace('"', [string]::Empty), $matches.Groups[2].Value.Replace('"', [string]::Empty), $matches.Groups[3].Value.Replace('"', [string]::Empty)
}

task Initialize-Directories { 
    if( Test-Path $output.TestsDirectory ) {
        Remove-Item -Path $output.TestsDirectory -Force -Recurse
    }
    if( Test-Path $ckpackage.OutputDirectory ) {
        Remove-Item -Path $ckpackage.OutputDirectory -Force -Recurse
    }
    
    $output.TestsDirectory = (New-Item -Force -ItemType Directory $output.TestsDirectory).FullName
    $ckpackage.OutputDirectory = (New-Item -Force -ItemType Directory $ckpackage.OutputDirectory).FullName
}

task NugetRestore {
    exec { 
        $packages.configFiles | % {
            $packagesDir = $packages.PackagesDirectory
            nuget restore $_.FullName -PackagesDirectory $packagesDir
        }
    }
}