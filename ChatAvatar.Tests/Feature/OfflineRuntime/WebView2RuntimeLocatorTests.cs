using System.IO;
using ChatAvatar.Component.UI;

namespace ChatAvatar.Tests.Feature.OfflineRuntime;

[TestClass]
public sealed class WebView2RuntimeLocatorTests
{
    [TestMethod]
    public void MissingBundledRuntimeAllowsDevelopmentEvergreenFallback()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            Assert.IsNull(WebView2RuntimeLocator.FindBundledRuntime(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void CompleteFixedRuntimeIsSelectedFromApplicationDirectory()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var runtime = Path.Combine(directory, WebView2RuntimeLocator.BundledRuntimeDirectoryName);
            Directory.CreateDirectory(Path.Combine(runtime, "locales"));
            foreach (var file in new[] { "msedgewebview2.exe", "icudtl.dat", "resources.pak" })
            {
                File.WriteAllBytes(Path.Combine(runtime, file), [1]);
            }

            Assert.AreEqual(runtime, WebView2RuntimeLocator.FindBundledRuntime(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void IncompleteBundledRuntimeFailsInsteadOfFallingBackOnline()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(directory, WebView2RuntimeLocator.BundledRuntimeDirectoryName));

            Assert.ThrowsExactly<InvalidDataException>(() => WebView2RuntimeLocator.FindBundledRuntime(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarWebViewRuntimeTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}