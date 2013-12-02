function Get-NUnitConsole {
    $nc = Get-ChildItem -Path .\ -File -Filter nunit-console.exe -Recurse
    if( $nc -ne $null ) {
        $nc = $nc[0]
    }

    return $nc
}

task Download-NUnitConsole -precondition { return (Get-NUnitConsole) -eq $null } {
    $packagesDir = $output.TestsDirectory
    exec { nuget install NUnit.Runners -OutputDirectory $packagesDir }
}

task Build-UnitTests {
  exec {
      $solution.TestProjects | % {
        $td = $output.TestsDirectory
        msbuild $_.FullName /nologo /p:Configuration=Debug /p:DownloadNuGetExe=false /p:RestorePackages=false /p:CKPackage=false /p:OutputPath=$td
      }
  }
}

task Run-NUnitTests {
    $nunitConsole = Get-NUnitConsole
    $dlls = (Get-ChildItem -Path $output.TestsDirectory -File $solution.TestDllsFormat).FullName
    $testFramework = $solution.TestFramework
    $workingDirectory = $output.TestsDirectory

    exec { & $nunitConsole.FullName /work:$workingDirectory /framework:$testFramework $dlls }
}