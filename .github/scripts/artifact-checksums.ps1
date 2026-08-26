<#
.SYNOPSIS
    Writes or verifies a SHA-256 manifest for an artifact handed between workflow jobs.

.DESCRIPTION
    The manifest cannot cover itself, so 'Write' emits the manifest's own SHA-256 as the
    'manifest-sha256' step output. Passing that value back through -ExpectedManifestHash lets a
    consuming job establish the integrity of the handoff out of band from the artifact itself.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('Write', 'Verify')]
    [string]$Mode,

    [Parameter(Mandatory)]
    [string]$ManifestPath,

    [Parameter(Mandatory)]
    [string[]]$Path,

    [string]$ExpectedManifestHash
)

$ErrorActionPreference = 'Stop'
$root = (Get-Location).Path

function Get-Entries {
    Get-ChildItem -Path $Path -Recurse -File |
        Sort-Object FullName |
        ForEach-Object {
            [pscustomobject]@{
                Hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
                Path = [IO.Path]::GetRelativePath($root, $_.FullName).Replace('\', '/')
            }
        }
}

if ($Mode -eq 'Write') {
    $lines = @(Get-Entries | ForEach-Object { "$($_.Hash)  $($_.Path)" })
    if ($lines.Count -eq 0) {
        throw "No files matched '$($Path -join ', ')'. Refusing to write an empty checksum manifest."
    }

    Set-Content -LiteralPath $ManifestPath -Value $lines -Encoding ascii
    $manifestHash = (Get-FileHash -LiteralPath $ManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()

    Write-Host "Recorded $($lines.Count) files in $ManifestPath (manifest SHA-256 $manifestHash)."
    if ($env:GITHUB_OUTPUT) {
        "manifest-sha256=$manifestHash" | Add-Content -LiteralPath $env:GITHUB_OUTPUT
    }
    return
}

if (-not (Test-Path -LiteralPath $ManifestPath)) {
    throw "Checksum manifest not found: $ManifestPath"
}

if ([string]::IsNullOrWhiteSpace($ExpectedManifestHash)) {
    throw "-ExpectedManifestHash is required in Verify mode; the manifest cannot vouch for itself."
}

$manifestHash = (Get-FileHash -LiteralPath $ManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ($manifestHash -ne $ExpectedManifestHash) {
    throw "Checksum manifest was altered in transit. Expected $ExpectedManifestHash but found $manifestHash."
}

$expected = @{}
foreach ($line in Get-Content -LiteralPath $ManifestPath) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $hash, $relative = $line -split '  ', 2
    $expected[$relative] = $hash
}

$actual = @{}
foreach ($entry in Get-Entries) { $actual[$entry.Path] = $entry.Hash }

$problems = @()
foreach ($relative in $expected.Keys) {
    if (-not $actual.ContainsKey($relative)) { $problems += "missing:    $relative" }
    elseif ($actual[$relative] -ne $expected[$relative]) { $problems += "modified:   $relative" }
}
foreach ($relative in $actual.Keys) {
    if (-not $expected.ContainsKey($relative)) { $problems += "unexpected: $relative" }
}

if ($problems.Count -gt 0) {
    throw "Artifact integrity check failed against ${ManifestPath}:`n$($problems -join "`n")"
}

Write-Host "Verified $($expected.Count) files against $ManifestPath (manifest SHA-256 $manifestHash)."