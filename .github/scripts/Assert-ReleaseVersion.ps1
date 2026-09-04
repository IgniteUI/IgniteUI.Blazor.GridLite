<#
.SYNOPSIS
    Validates that a release tag is a package version supported by the release workflow.

.DESCRIPTION
    Release tag names are untrusted input. This script accepts the repository's existing bare SemVer
    convention and rejects values that could be interpreted as PowerShell when used by later jobs.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

$coreIdentifier = '(?:0|[1-9][0-9]*)'
$prereleaseIdentifier = '(?:0|[1-9][0-9]*|[0-9A-Za-z-]*[A-Za-z-][0-9A-Za-z-]*)'
$supportedVersionPattern = "^$coreIdentifier\.$coreIdentifier\.$coreIdentifier(?:-$prereleaseIdentifier(?:\.$prereleaseIdentifier)*)?$"

if ($Version -notmatch $supportedVersionPattern) {
    throw "Release tag '$Version' must be a bare SemVer package version such as '1.2.3' or '1.2.3-prerelease.4'. A 'v' prefix and build metadata are not supported."
}

Write-Host "Validated release version $Version."
