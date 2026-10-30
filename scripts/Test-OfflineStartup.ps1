$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$solutionPath = Join-Path $repositoryRoot 'ChatAvatar.sln'
$applicationPath = Join-Path $repositoryRoot 'ChatAvatar.TestApp\bin\Debug\net10.0-windows\ChatAvatar.TestApp.exe'

Push-Location $repositoryRoot
try {
    dotnet build $solutionPath --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw 'The offline solution build failed.'
    }

    dotnet test $solutionPath --no-build --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw 'The offline test run failed.'
    }

    if (-not (Test-Path $applicationPath)) {
        throw "The WPF test application was not found at $applicationPath."
    }

    $application = Start-Process -FilePath $applicationPath -PassThru
    try {
        if (-not $application.WaitForInputIdle(10000)) {
            throw 'The WPF test application did not become idle within 10 seconds.'
        }

        if ($application.HasExited) {
            throw 'The WPF test application exited during startup.'
        }

        Write-Output 'WPF process reached input idle; offline startup check passed.'
    }
    finally {
        if (-not $application.HasExited) {
            $null = $application.CloseMainWindow()
            if (-not $application.WaitForExit(3000)) {
                Stop-Process -Id $application.Id -Force
            }
        }
    }
}
finally {
    Pop-Location
}