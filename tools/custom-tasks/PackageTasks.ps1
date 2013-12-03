if( (Get-Module CK.Package.Components) -eq $null ) {
    Import-Module .\tools\custom-modules\CK-Package\CK.Package.Components.psd1
}

task CKPackage -depends Initialize-Directories {
    Invoke-CKPackage -SolutionDirectory (get-item $ckpackage.SolutionDirectory) -OutputDirectory (get-item $outputDirectory)
}