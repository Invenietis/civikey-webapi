Import-Module .\tools\custom-modules\CK-Package4PS\CK.Package.Components.psd1

task CKPackage -depends Initialize-Directories {
    exec { Invoke-CKPackage -SolutionDirectory (get-item $ckpackage.SolutionDirectory) -OutputDirectory (get-item $ckpackage.OutputDirectory) }
}