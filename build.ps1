framework "4.0"

Include .\tools\custom-tasks\SharedAssemblyInfoTasks.ps1
Include .\tools\custom-tasks\UtilitiesTasks.ps1
Include .\tools\custom-tasks\BuildTasks.ps1
Include .\tools\custom-tasks\UnitTestTasks.ps1
Include .\tools\custom-tasks\PackageTasks.ps1

properties {
    $solution = @{}
    # The directory that contains the solution (where we can find the .sln file).
    $solution.Directory = "src"
    # The solution file name.
    $solution.FileName = "CiviKey.WebApi" 
    # The shared assembly info file.
    $solution.SharedAssemblyInfoFile = Get-Item (Join-Path $solution.Directory -ChildPath "SharedAssemblyInfo.cs")
    # All tests projects found by Get-Children and a filter.
    $solution.TestProjects = Get-ChildItem -Path $solution.Directory -File -Filter "*.Tests.csproj" -Recurse
    # The format used to locate the dlls to process for the tests.
    $solution.TestDllsFormat = "*.Tests.dll"
    # The framework used to run the unit tests.
    $solution.TestFramework = "net-4.0" 
    
    $output = @{}
    # The directory where the tests are going to be build.
    $output.TestsDirectory = ".\output\tests"

    $packages = @{}
    # The directory used by nuget to output the restored packages.
    $packages.PackagesDirectory = Join-Path $solution.Directory "packages"
    # All packages.config file of the solution. 
    # Used by nuget to restore the missing packages in order to build the solution.
    $packages.ConfigFiles = Get-ChildItem -Path $solution.Directory -Filter "packages.config" -Recurse

    $ckpackage = @{}
    # The solution directory where the CKPackage cmdlet can find a Solution.ck file.
    $ckpackage.SolutionDirectory = ".\"
    # The directory where the CKPackage cmdlet can output the produced package.
    $ckpackage.OutputDirectory = ".\output\package"
}

task Build -depends Initialize-Directories, NugetRestore, Update-SharedAssemblyInfo, Build-Solution-Debug, Build-Solution-Release, Reset-SharedAssemblyInfo

task UnitTest -depends Initialize-Directories, Download-NUnitConsole, NugetRestore, Build-UnitTests, Run-NUnitTests

task Package -depends Initialize-Directories, CKPackage

task Default -depends Build, UnitTest, Package