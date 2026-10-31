using System.IO;

namespace ChatAvatar.Tests.Featur.OfflineRuntime;

[TestClass]
public sealed class WebViewLocalAssetTests
{
    [TestMethod]
    public async Task ThreeJsAssetsAreCopiedLocallyAndDisallowNetworkResources()
    {
        var pagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "index.html");
        var scenePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "avatar-scene.js");
        var facePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "face-avatar.js");
        var threePath = Path.Combine(AppContext.BaseDirectory, "Assets", "ThreeJs", "three.module.js");

        Assert.IsTrue(File.Exists(pagePath), $"Expected local WebView page at {pagePath}.");
        Assert.IsTrue(File.Exists(scenePath), $"Expected local Three.js scene at {scenePath}.");
        Assert.IsTrue(File.Exists(facePath), $"Expected local face avatar at {facePath}.");
        Assert.IsTrue(File.Exists(threePath), $"Expected local Three.js module at {threePath}.");
        var page = await File.ReadAllTextAsync(pagePath);
        var scene = await File.ReadAllTextAsync(scenePath);
        var face = await File.ReadAllTextAsync(facePath);

        StringAssert.Contains(page, "default-src 'none'");
        StringAssert.Contains(page, "connect-src 'none'");
        StringAssert.Contains(scene, "PerspectiveCamera");
        StringAssert.Contains(scene, "ResizeObserver");
        StringAssert.Contains(scene, "../ThreeJs/three.module.js");
        StringAssert.Contains(scene, "./face-avatar.js");
        StringAssert.Contains(face, "SphereGeometry");
        StringAssert.Contains(face, "setViseme");
        Assert.IsFalse(page.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(page.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(scene.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(scene.Contains("http://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(face.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(face.Contains("http://", StringComparison.OrdinalIgnoreCase));
    }
}