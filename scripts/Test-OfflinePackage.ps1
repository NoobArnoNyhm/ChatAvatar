param(
    [string]$PackagePath = 'artifacts/offline-win-x64'
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$resolvedPackagePath = if ([IO.Path]::IsPathRooted($PackagePath)) {
    $PackagePath
} else {
    Join-Path $repositoryRoot $PackagePath
}

$manifestPath = Join-Path $resolvedPackagePath 'offline-deployment.json'
$applicationPath = Join-Path $resolvedPackagePath 'ChatAvatar.TestApp.exe'
$runtimePath = Join-Path $resolvedPackagePath 'WebView2FixedRuntime'
if (-not (Test-Path $manifestPath -PathType Leaf)) {
    throw "Offline deployment manifest not found: $manifestPath"
}

$manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
if (-not $manifest.selfContained -or $manifest.externalServicesRequired) {
    throw 'The package manifest does not describe a self-contained offline application.'
}

if (-not (Test-Path $applicationPath -PathType Leaf)) {
    throw "Published application executable not found: $applicationPath"
}

$requiredFiles = @(
    'msedgewebview2.exe',
    'icudtl.dat',
    'resources.pak',
    'Assets\WebView\index.html',
    'Assets\WebView\avatar-scene.js',
    'Assets\ThreeJs\three.module.js',
    'Assets\Avatars\sample-face.avatar.json',
    'Assets\Avatars\sample-image-based.avatar.json',
    'Assets\Avatars\sample-abstract.avatar.json'
)
foreach ($file in $requiredFiles) {
    $path = if ($file.StartsWith('Assets\', [StringComparison]::OrdinalIgnoreCase)) {
        Join-Path $resolvedPackagePath $file
    } else {
        Join-Path $runtimePath $file
    }
    if (-not (Test-Path $path)) {
        throw "The offline package is missing '$file'."
    }
}
if (-not (Test-Path (Join-Path $runtimePath 'locales') -PathType Container)) {
    throw 'The offline package is missing the WebView2 locales directory.'
}

$process = Start-Process -FilePath $applicationPath -WorkingDirectory $resolvedPackagePath -PassThru
try {
    $ready = $process.WaitForInputIdle(15000)
    $process.Refresh()
    if ($process.HasExited -or -not $ready) {
        throw 'The packaged WPF application did not reach its message loop.'
    }

    Write-Output "Offline package smoke test passed; WebView2 $($manifest.webView2Runtime)."
}
finally {
    if (-not $process.HasExited) {
        $null = $process.CloseMainWindow()
        if (-not $process.WaitForExit(3000)) {
            Stop-Process -Id $process.Id -Force
        }
    }
}