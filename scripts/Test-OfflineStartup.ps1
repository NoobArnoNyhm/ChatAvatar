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

    & (Join-Path $PSScriptRoot 'Test-AvatarRendering.ps1') -ApplicationPath $applicationPath
}
finally {
    Pop-Location
}