using System.IO;

namespace ChatAvatar.Tests.Featur.OfflineRuntime;

[TestClass]
public sealed class WebViewLocalAssetTests
{
    [TestMethod]
    public async Task AvatarPageIsCopiedLocallyAndDisallowsNetworkResources()
    {
        var pagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "index.html");

        Assert.IsTrue(File.Exists(pagePath), $"Expected local WebView page at {pagePath}.");
        var page = await File.ReadAllTextAsync(pagePath);

        StringAssert.Contains(page, "default-src 'none'");
        StringAssert.Contains(page, "connect-src 'none'");
        Assert.IsFalse(page.Contains("https://", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(page.Contains("http://", StringComparison.OrdinalIgnoreCase));
    }
}