<#
.SYNOPSIS
    Generates a CycloneDX SBOM for the project and validates its licence and supplier coverage.

.DESCRIPTION
    Ships alongside the SPDX documents because the two derive their licence data from different places
    and neither is complete on its own.

    sbom-tool resolves licences from ClearlyDefined, a network service that harvests package definitions
    on demand and degrades to NOASSERTION whenever it is slow or unavailable. CycloneDX reads the licence
    expression and authors straight out of each package's own nuspec in the restore cache, so it produces
    the same answer every run without a network round trip.

    The two also disagree usefully. ClearlyDefined scans licence file text and can name a licence that a
    nuspec only points at by filename; CycloneDX reports authors, which the SPDX documents leave as
    NOASSERTION for every dependency. Publishing both is what makes the licence picture complete.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ProjectPath,

    [Parameter(Mandatory)]
    [string]$PackageId,

    [Parameter(Mandatory)]
    [string]$PackageVersion,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    # Resolves licences for packages whose nuspec points at a licence file instead of an SPDX expression.
    # In Actions, supply secrets.GITHUB_TOKEN; without it those packages are left unlicensed.
    [string]$GitHubBearerToken,

    [ValidateRange(0, 1)]
    [double]$MinimumLicenseCoverage = 0.9
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

$arguments = @(
    $ProjectPath
    '--output', $OutputDirectory
    '--json'
    '--set-name', $PackageId
    '--set-version', $PackageVersion
    '--set-type', 'Library'
    '--exclude-dev'
    '--include-license-text'
)

if ($GitHubBearerToken) {
    $env:CYCLONEDX_GITHUB_BEARER_TOKEN = $GitHubBearerToken
    $arguments += '--enable-github-licenses'
}
else {
    Write-Warning 'No GitHub token supplied; packages that declare a licence file rather than an SPDX expression will be left unlicensed.'
}

try {
    dotnet tool run dotnet-CycloneDX -- @arguments
}
finally {
    Remove-Item Env:\CYCLONEDX_GITHUB_BEARER_TOKEN -ErrorAction SilentlyContinue
}

$bomPath = Join-Path $OutputDirectory 'bom.json'
if (-not (Test-Path -LiteralPath $bomPath -PathType Leaf)) {
    throw "CycloneDX did not produce a document at $bomPath."
}

$bom = Get-Content -LiteralPath $bomPath -Raw | ConvertFrom-Json

# actions/attest only recognises a CycloneDX document that carries all three of these.
foreach ($required in 'bomFormat', 'specVersion', 'serialNumber') {
    if (-not $bom.$required) {
        throw "CycloneDX document is missing '$required', so it cannot be consumed as an SBOM predicate."
    }
}

$components = @($bom.components)
if ($components.Count -eq 0) {
    throw 'CycloneDX document contains no components.'
}

$licensed = @($components | Where-Object { $_.licenses })
# CycloneDX models 'authors' (people, from the nuspec author metadata) and 'supplier' (an organisation)
# separately; cyclonedx-dotnet only ever populates the former, so this is reported as author coverage.
$authored = @($components | Where-Object { $_.authors })
$coverage = $licensed.Count / $components.Count

$renamedPath = Join-Path $OutputDirectory "$PackageId.$PackageVersion.cdx.json"
Move-Item -LiteralPath $bomPath -Destination $renamedPath -Force

$checksum = (Get-FileHash -LiteralPath $renamedPath -Algorithm SHA256).Hash.ToLowerInvariant()
Set-Content -LiteralPath "$renamedPath.sha256" -Value $checksum -NoNewline

Write-Host "CycloneDX $($bom.specVersion): $($components.Count) components, $($licensed.Count) licensed, $($authored.Count) with an author, $(@($bom.dependencies).Count) dependency edges."

if ($coverage -lt $MinimumLicenseCoverage) {
    $unlicensed = @($components | Where-Object { -not $_.licenses } | ForEach-Object { "$($_.name)@$($_.version)" })
    Write-Warning "Only $([math]::Round($coverage * 100))% of components carry a licence (threshold $([math]::Round($MinimumLicenseCoverage * 100))%). Unresolved: $($unlicensed -join ', ')"
}
