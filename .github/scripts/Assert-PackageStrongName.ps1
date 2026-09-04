<#
.SYNOPSIS
    Verifies the strong names of the assemblies inside a packed NuGet package.

.DESCRIPTION
    The assemblies validated after signing are the ones in bin/. This extracts the package that will
    actually ship and re-checks those bytes, so a pack step that picked up a different build is caught.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PackagePath,

    [Parameter(Mandatory)]
    [string]$ExpectedPublicKeyPath,

    [string]$WorkingDirectory = (Join-Path ([System.IO.Path]::GetTempPath()) 'strong-name-validation')
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) {
    throw "NuGet package not found: $PackagePath"
}

$extractPath = Join-Path $WorkingDirectory 'package'
# Expand-Archive only accepts .zip, so the package is copied under a name it will open.
$archivePath = Join-Path $WorkingDirectory 'package.zip'

try {
    New-Item -ItemType Directory -Path $WorkingDirectory -Force | Out-Null
    Copy-Item -LiteralPath $PackagePath -Destination $archivePath -Force
    Expand-Archive -LiteralPath $archivePath -DestinationPath $extractPath -Force

    & (Join-Path $PSScriptRoot 'Assert-AssemblyStrongName.ps1') -Path $extractPath -ExpectedPublicKeyPath $ExpectedPublicKeyPath
}
finally {
    Remove-Item -LiteralPath $WorkingDirectory -Recurse -Force -ErrorAction SilentlyContinue
}
