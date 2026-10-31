param(
    [Parameter(Mandatory = $true)]
    [string]$WebView2FixedRuntimePath,
    [string]$OutputPath = 'artifacts/offline-win-x64'
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$projectPath = Join-Path $repositoryRoot 'ChatAvatar.TestApp\ChatAvatar.TestApp.csproj'
$runtimeSource = (Resolve-Path $WebView2FixedRuntimePath).Path
$resolvedOutputPath = if ([IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath
} else {
    Join-Path $repositoryRoot $OutputPath
}

$requiredRuntimeFiles = @('msedgewebview2.exe', 'icudtl.dat', 'resources.pak')
foreach ($file in $requiredRuntimeFiles) {
    if (-not (Test-Path (Join-Path $runtimeSource $file) -PathType Leaf)) {
        throw "The WebView2 Fixed Version runtime is missing '$file'."
    }
}
if (-not (Test-Path (Join-Path $runtimeSource 'locales') -PathType Container)) {
    throw 'The WebView2 Fixed Version runtime is missing the locales directory.'
}

New-Item -ItemType Directory -Force -Path $resolvedOutputPath | Out-Null
Push-Location $repositoryRoot
try {
    dotnet publish $projectPath `
        --configuration Release `
        --no-restore `
        --property:PublishProfile=Offline-x64 `
        --output $resolvedOutputPath
    if ($LASTEXITCODE -ne 0) {
        throw 'The offline WPF publish failed. Restore the win-x64 target before disconnecting from the network.'
    }

    $runtimeDestination = Join-Path $resolvedOutputPath 'WebView2FixedRuntime'
    New-Item -ItemType Directory -Force -Path $runtimeDestination | Out-Null
    Copy-Item -Path (Join-Path $runtimeSource '*') -Destination $runtimeDestination -Recurse -Force

    $requiredAssets = @(
        'Assets\WebView\index.html',
        'Assets\WebView\avatar-scene.js',
        'Assets\ThreeJs\three.module.js',
        'Assets\Avatars\sample-face.avatar.json',
        'Assets\Avatars\sample-image-based.avatar.json',
        'Assets\Avatars\sample-abstract.avatar.json'
    )
    foreach ($asset in $requiredAssets) {
        if (-not (Test-Path (Join-Path $resolvedOutputPath $asset) -PathType Leaf)) {
            throw "The offline publish is missing runtime asset '$asset'."
        }
    }

    $manifest = [ordered]@{
        application = 'ChatAvatar.TestApp'
        runtimeIdentifier = 'win-x64'
        selfContained = $true
        webView2Runtime = [Diagnostics.FileVersionInfo]::GetVersionInfo(
            (Join-Path $runtimeSource 'msedgewebview2.exe')).FileVersion
        runtimeDirectory = 'WebView2FixedRuntime'
        externalServicesRequired = $false
    }
    $manifestPath = Join-Path $resolvedOutputPath 'offline-deployment.json'
    ConvertTo-Json $manifest -Depth 4 | Set-Content -Path $manifestPath -Encoding utf8

    Write-Output "Offline package created at '$resolvedOutputPath'."
}
finally {
    Pop-Location
}