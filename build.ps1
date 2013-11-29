framework "4.0"

Include .\tools\custom-tasks\SharedAssemblyInfoTasks.ps1
Include .\tools\custom-tasks\UtilitiesTasks.ps1
Include .\tools\custom-tasks\BuildTasks.ps1
Include .\tools\custom-tasks\UnitTestTasks.ps1
Include .\tools\custom-tasks\PackageTasks.ps1

properties {
    $solution = @{}
    $solution.Directory = "src"
    $solution.FileName = "CiviKey.WebApi"
    $solution.SharedAssemblyInfoFile = Get-Item (Join-Path $solution.Directory -ChildPath "SharedAssemblyInfo.cs")
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

task Build -depends Initialize-Directories, NugetRestore, Update-SharedAssemblyInfo, Build-Solution-Debug, Build-Solution-Release, Reset-SharedAssemblyInfo

task UnitTest -depends Initialize-Directories, Download-NUnitConsole, NugetRestore, Build-UnitTests, Run-NUnitTests

task Package -depends Initialize-Directories, CKPackage

task Default -depends Build, UnitTest, Package