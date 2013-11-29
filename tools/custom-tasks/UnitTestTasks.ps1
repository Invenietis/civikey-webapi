task Download-NUnitConsole {
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
    $nunitConsole = Get-ChildItem -Path $output.TestsDirectory -File -Filter nunit-console.exe -Recurse
    $dlls = (Get-ChildItem -Path $output.TestsDirectory -File $solution.TestDllsFormat).FullName
    $testFramework = $solution.TestFramework

    exec { & $nunitConsole.FullName $dlls /framework:$testFramework }
}