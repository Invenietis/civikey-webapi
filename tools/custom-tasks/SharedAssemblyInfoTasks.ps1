Import-Module .\tools\custom-modules\git-utils.ps1

task Update-SharedAssemblyInfo {
    exec {
        # copy source file
        $source = $solution.SharedAssemblyInfoFile

        assert( !(Test-Path (join-path $source.Directory.FullName "SharedAssemblyInfo.source.cs")) ) "The SharedAssemblyInfo file has already been updated, reset it before re-update"

        Copy-Item $solution.SharedAssemblyInfoFile -Destination (join-path $source.Directory.FullName "SharedAssemblyInfo.source.cs")
        
        # read the content
        $content = Get-Content -Encoding UTF8 $source

        assert (($content -match "AssemblyInformationalVersion").Length -eq 0) "The AssemblyInformationalVersion shouldn't be already defined"
        assert (($content -match "AssemblyVersion").Length -eq 0) "The AssemblyVersion shouldn't be already defined"
        assert (($content -match "AssemblyFileVersion").Length -eq 0) "The AssemblyFileVersion shouldn't be already defined"

        $gitInformations = Get-GitInformationsToPlant
        $currentVersion = Get-CurrentVersion
        $informationalVersionAttribute = ('{0}[assembly: AssemblyInformationalVersion( "{1}-{2}" )]' -f [Environment]::NewLine, $currentVersion, $gitInformations)
        $versionAttribute = ('{0}[assembly: AssemblyVersion( "{1}" )]' -f [Environment]::NewLine, $currentVersion)
        $fileVersionAttribute = ('{0}[assembly: AssemblyFileVersion( "{1}" )]' -f [Environment]::NewLine, $currentVersion)

        Add-Content -Encoding UTF8 $source "$informationalVersionAttribute $versionAttribute $fileVersionAttribute"
    }
}

task Reset-SharedAssemblyInfo {
    exec {
        $source = $solution.SharedAssemblyInfoFile
        $copy = Get-Item (join-path $source.Directory.FullName "SharedAssemblyInfo.source.cs")
        
        Remove-Item $source
        Move-Item $copy $source
    }
}