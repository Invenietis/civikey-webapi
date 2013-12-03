function Get-NUnitConsole {
    $nc = Get-ChildItem -Path .\ -File -Filter nunit-console.exe -Recurse
    if( $nc -ne $null ) {
        $nc = $nc[0]
    }

    return $nc
}

task Download-NUnitConsole -precondition { return (Get-NUnitConsole) -eq $null } {
    $nunitConsoleDir = $packages.PackagesDirectory
    exec { nuget install NUnit.Runners -OutputDirectory $nunitConsoleDir }
}

task Build-UnitTests {
  exec {
      $solution.TestProjects | % {
        msbuild $_.FullName /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:MvcBuildViews=false /p:CKPackage=false
      }
  }
}

task Run-NUnitTests {
    $nunitConsole = Get-NUnitConsole

    $dlls = $solution.TestProjects | % { 
        $path = Join-Path $_.Directory.FullName -ChildPath (join-path bin Debug) 
        $dllName = "{0}.dll" -f $_.Name.Substring(0, $_.Name.Length-$_.Extension.Length)
        return (Get-ChildItem -Path $path -File $dllName).FullName 
    }

    $testFramework = $solution.TestFramework
    $resultFile = join-path $outputDirectory TestResults.xml

    exec { & $nunitConsole.FullName /result:$resultFile /framework:$testFramework $dlls }
}