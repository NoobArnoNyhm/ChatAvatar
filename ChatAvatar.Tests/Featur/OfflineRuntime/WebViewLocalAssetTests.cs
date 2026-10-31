using System.IO;

namespace ChatAvatar.Tests.Featur.OfflineRuntime;

[TestClass]
public sealed class WebViewLocalAssetTests
{
    [TestMethod]
    public async Task ThreeJsAssetsAreCopiedLocallyAndDisallowNetworkResources()
    {
        var pagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "index.html");
        var diagnosticsPath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "webview-diagnostics.js");
        var scenePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "avatar-scene.js");
        var facePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "face-avatar.js");
        var imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "image-based-avatar.js");
        var abstractPath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "abstract-avatar.js");
        var threePath = Path.Combine(AppContext.BaseDirectory, "Assets", "ThreeJs", "three.module.js");
        var threeCorePath = Path.Combine(AppContext.BaseDirectory, "Assets", "ThreeJs", "three.core.js");

        Assert.IsTrue(File.Exists(pagePath), $"Expected local WebView page at {pagePath}.");
        Assert.IsTrue(File.Exists(diagnosticsPath), $"Expected local WebView diagnostics at {diagnosticsPath}.");
        Assert.IsTrue(File.Exists(scenePath), $"Expected local Three.js scene at {scenePath}.");
        Assert.IsTrue(File.Exists(facePath), $"Expected local face avatar at {facePath}.");
        Assert.IsTrue(File.Exists(imagePath), $"Expected local image-based avatar at {imagePath}.");
        Assert.IsTrue(File.Exists(abstractPath), $"Expected local abstract avatar at {abstractPath}.");
        Assert.IsTrue(File.Exists(threePath), $"Expected local Three.js module at {threePath}.");
        Assert.IsTrue(File.Exists(threeCorePath), $"Expected local Three.js core module at {threeCorePath}.");
        var page = await File.ReadAllTextAsync(pagePath);
        var diagnostics = await File.ReadAllTextAsync(diagnosticsPath);
        var scene = await File.ReadAllTextAsync(scenePath);
        var face = await File.ReadAllTextAsync(facePath);
        var image = await File.ReadAllTextAsync(imagePath);
        var abstractAvatar = await File.ReadAllTextAsync(abstractPath);
        var threeModule = await File.ReadAllTextAsync(threePath);

        StringAssert.Contains(page, "default-src 'none'");
        StringAssert.Contains(page, "connect-src 'none'");
        StringAssert.Contains(page, "webview-diagnostics.js");
        StringAssert.Contains(scene, "sceneReady");
        StringAssert.Contains(diagnostics, "unhandledrejection");
        StringAssert.Contains(diagnostics, "No 3D frame was rendered");
        StringAssert.Contains(scene, "renderer.render(scene, camera)");
        StringAssert.Contains(scene, "PerspectiveCamera");
        StringAssert.Contains(scene, "ResizeObserver");
        StringAssert.Contains(scene, "../ThreeJs/three.module.js");
        StringAssert.Contains(scene, "setavatarappearance");
        StringAssert.Contains(scene, "./face-avatar.js");
        StringAssert.Contains(threeModule, "from './three.core.js'");
        StringAssert.Contains(face, "SphereGeometry");
        StringAssert.Contains(face, "function setAppearance");
        StringAssert.Contains(face, "setViseme");
        StringAssert.Contains(image, "ExtrudeGeometry");
        StringAssert.Contains(image, "TorusGeometry");
        StringAssert.Contains(image, "CanvasTexture");
        StringAssert.Contains(image, "TextureLoader");
        StringAssert.Contains(image, "function setAppearance");
        StringAssert.Contains(image, "speechEnergy");
        StringAssert.Contains(abstractAvatar, "PointLight");
        StringAssert.Contains(abstractAvatar, "function setAppearance");
        StringAssert.Contains(abstractAvatar, "setAudioLevel");
        Assert.IsFalse(page.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(page.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(scene.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(scene.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(face.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(face.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(image.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(image.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(abstractAvatar.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(abstractAvatar.Contains("http://", StringComparison.OrdinalIgnoreCase));
    }
}