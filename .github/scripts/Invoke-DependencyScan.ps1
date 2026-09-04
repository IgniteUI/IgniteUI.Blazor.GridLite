<#
.SYNOPSIS
    Records the known vulnerabilities in the dependencies the package ships.

.DESCRIPTION
    Advisory by design. A finding is annotated and attached to the release as evidence but never holds
    up the publish; the blocking gate for newly introduced vulnerable dependencies is a PR-time
    dependency-review check. A scan that fails to run, however, is an error: silence must not be
    mistaken for a clean result.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ProjectPath,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [string]$PackageId,

    [string]$Version,

    [string]$SummaryPath = $env:GITHUB_STEP_SUMMARY
)

$ErrorActionPreference = 'Stop'
# The scan's exit code is inspected explicitly so a failure can be reported with context.
$PSNativeCommandUseErrorActionPreference = $false

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$reportPath = Join-Path $OutputDirectory 'nuget-vulnerable.json'

dotnet restore $ProjectPath | Out-Null
if ($LASTEXITCODE -ne 0) {
    throw "dotnet restore failed for $ProjectPath with exit code $LASTEXITCODE."
}

dotnet list $ProjectPath package --vulnerable --include-transitive --format json --output-version 1 |
    Set-Content -LiteralPath $reportPath -Encoding utf8
if ($LASTEXITCODE -ne 0) {
    throw "dotnet list package --vulnerable failed with exit code $LASTEXITCODE."
}

$report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
if ($report.version -ne 1 -or -not $report.projects) {
    throw 'dotnet list package did not produce a valid version 1 JSON report.'
}

$findings = @(
    foreach ($project in $report.projects) {
        # A framework with no findings omits the package arrays entirely, so null entries are dropped.
        foreach ($framework in @($project.frameworks | Where-Object { $_ })) {
            $packages = @($framework.topLevelPackages) + @($framework.transitivePackages)
            foreach ($package in @($packages | Where-Object { $_ })) {
                foreach ($vulnerability in @($package.vulnerabilities | Where-Object { $_ })) {
                    [pscustomobject]@{
                        Framework   = $framework.framework
                        Package     = $package.id
                        Resolved    = $package.resolvedVersion
                        Severity    = $vulnerability.severity
                        AdvisoryUrl = $vulnerability.advisoryurl
                    }
                }
            }
        }
    }
)

$table = if ($findings.Count -gt 0) {
    $findings | Sort-Object Severity, Package, Framework | Format-Table -AutoSize | Out-String -Width 200
}
else {
    'No vulnerable shipped dependencies reported.'
}

$table | Set-Content -LiteralPath (Join-Path $OutputDirectory 'nuget-vulnerable.txt') -Encoding utf8
Write-Host $table

if (-not $SummaryPath) {
    return
}

$summary = @(
    '### Dependency vulnerability scan'
    ''
    'Advisory only - findings are recorded but do not block this release.'
    ''
    '<details><summary>dotnet list package --vulnerable --include-transitive</summary>'
    ''
    '```'
    $table.TrimEnd()
    '```'
    ''
    '</details>'
    ''
)

if ($findings.Count -gt 0) {
    $label = "$PackageId $Version".Trim()
    Write-Host "::warning title=Vulnerable dependencies reported::$label was released with $($findings.Count) dependency advisories outstanding. See the run summary and the dependency-scan release asset."
    $summary += "> [!WARNING]`n> $($findings.Count) vulnerable dependencies were reported for this release. Review the scan output above and open a servicing issue if a fix is required."
}
else {
    $summary += '> [!NOTE]`n> No vulnerable shipped dependencies reported.'
}

$summary | Add-Content -LiteralPath $SummaryPath
