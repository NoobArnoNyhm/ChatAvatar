param(
    [Parameter(Mandatory = $true)]
    [string]$ApplicationPath,
    [int]$TimeoutMilliseconds = 20000
)

$ErrorActionPreference = 'Stop'
$resolvedApplicationPath = (Resolve-Path $ApplicationPath).Path
$applicationDirectory = Split-Path $resolvedApplicationPath -Parent
$reportPath = Join-Path $env:TEMP "ChatAvatar-render-$([guid]::NewGuid().ToString('N')).json"
$previousReportPath = $env:CHATAVATAR_RENDER_SMOKE_REPORT
$env:CHATAVATAR_RENDER_SMOKE_REPORT = $reportPath
$process = $null

try {
    $process = Start-Process `
        -FilePath $resolvedApplicationPath `
        -WorkingDirectory $applicationDirectory `
        -ArgumentList '--render-smoke-test' `
        -PassThru

    if (-not $process.WaitForExit($TimeoutMilliseconds)) {
        Stop-Process -Id $process.Id -Force
        throw "The avatar scene did not render within $TimeoutMilliseconds ms."
    }

    if (-not (Test-Path $reportPath -PathType Leaf)) {
        throw "The render smoke test exited with code $($process.ExitCode) without a scene report."
    }

    $report = Get-Content $reportPath -Raw | ConvertFrom-Json
    if (-not $report.sceneReady) {
        throw "The WebView2 avatar renderer failed: $($report.details)"
    }

    if ($process.ExitCode -ne 0) {
        throw "The render smoke test returned exit code $($process.ExitCode)."
    }

    Write-Output "WebView2 rendered the first 3D avatar frame: $($report.details)"
}
finally {
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }

    if ($null -eq $previousReportPath) {
        Remove-Item Env:CHATAVATAR_RENDER_SMOKE_REPORT -ErrorAction SilentlyContinue
    } else {
        $env:CHATAVATAR_RENDER_SMOKE_REPORT = $previousReportPath
    }

    Remove-Item $reportPath -Force -ErrorAction SilentlyContinue
}