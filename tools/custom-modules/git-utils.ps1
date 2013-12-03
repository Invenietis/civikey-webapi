function Get-GitCurrentBranchName() {
    return & git symbolic-ref --short -q HEAD
}
function Get-GitCurrentSha1() {
    return & git rev-parse HEAD
}
function Get-GitCurrentFileInChange() {
    return & git status --porcelain | ? { $_ -Match " M " } | % { ([string]$_).TrimStart(" M ") } | % { Get-Item $_ }
}
function Get-GitCurrentFileInChangeCount([bool]$countAllChanges) {
    $count = 0
    if( !$countAllChanges ) {
        foreach( $c in Get-GitCurrentFileInChange ) {
            $count++
        }
    }
    else {
        & git status --porcelain | % { $count++ }
    }

    return $count
}
function Get-GitCurrentCommitCountAheadOfOrigin(){
    $currentBranch = Get-GitCurrentBranchName
    $remoteName = & git config --get branch.$currentBranch.remote
    if( $remoteName -ne $null ) {
        $refName = & git config --get branch.$currentBranch.merge

        $remoteRefParts = $refName.Split('/')

        $remoteRefName = "{0}/{1}" -f $remoteName, $remoteRefParts[$remoteRefParts.Length-1]

        return ((& git rev-list ("{0}..{1}" -f $remoteRefName, $currentBranch)) | measure).Count
    }
    return 0
}

function Get-GitInformationsToPlant() {
    $branchName = Get-GitCurrentBranchName
    $sha1 = Get-GitCurrentSha1
    $countAhead = Get-GitCurrentCommitCountAheadOfOrigin
    $currentChanges = Get-GitCurrentFileInChangeCount $true
    $currentChangesFormat = ""

    if( $currentChanges -gt 0 ) {
        $currentChangesFormat = "+{0}M" -f $currentChanges
    }

    return "{0}/{1}{2}/{3}" -f $branchName, $countAhead, $currentChangesFormat, $sha1
}