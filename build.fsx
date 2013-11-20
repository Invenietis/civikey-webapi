// include Fake lib
#r @"tools\FAKE\tools\FakeLib.dll"

open Fake

// Directories
let testDir   = @".\output\test\"
let packagesDir = @".\output\packages\"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [testDir; packagesDir]
)

Target "DeleteUselessDirs" (fun _ -> 
    DeleteDir testDir
)

Target "Compile-Application" (fun _ ->
    MSBuild null "Build" ["Configuration", "Release"; "CKPackage", "false"; "DownloadNuGetExe", "true"; "RestorePackages", "true"] [@".\src\CiviKey.WebApi.sln"]
    |> Log "AppBuild-Output: "
)

Target "Compile-UnitTests" (fun _ ->
    !! @".\src\**\*.Tests.csproj"
    |> MSBuild testDir "Build" ["Configuration", "Debug"; "DownloadNuGetExe", "true"; "RestorePackages", "true"]
    |> Log "TestBuild-Output"
)

Target "Run-NUnitTest" (fun _ ->  
    [(testDir + @"\CiviKey.WebApi.Tests.dll")]
    |> NUnit (fun p -> 
                 {p with 
                   DisableShadowCopy = true; 
                   Framework = "4.5";
                   OutputFile = testDir + @"TestResults.xml"})
)

Target "Default" DoNothing

"Clean"
==> "Compile-Application"
==> "Compile-UnitTests"
==> "Run-NUnitTest"
==> "DeleteUselessDirs"
==> "Default"

Run "Default"