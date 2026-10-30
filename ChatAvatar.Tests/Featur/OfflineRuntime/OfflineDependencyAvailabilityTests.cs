using Microsoft.Web.WebView2.Core;
using System.Speech.Synthesis;

namespace ChatAvatar.Tests.Featur.OfflineRuntime;

[TestClass]
public sealed class OfflineDependencyAvailabilityTests
{
    [TestMethod]
    public void WebView2SdkAssemblyIsAvailableLocally()
    {
        Assert.AreEqual("Microsoft.Web.WebView2.Core", typeof(CoreWebView2Environment).Assembly.GetName().Name);
    }

    [TestMethod]
    public void SystemSpeechAssemblyIsAvailableLocally()
    {
        Assert.AreEqual("System.Speech", typeof(SpeechSynthesizer).Assembly.GetName().Name);
    }

    [TestMethod]
    public void ThreeJsModuleIsCopiedAsALocalRuntimeAsset()
    {
        var modulePath = Path.Combine(AppContext.BaseDirectory, "Assets", "ThreeJs", "three.module.js");

        Assert.IsTrue(File.Exists(modulePath), $"Expected local Three.js module at {modulePath}.");
        Assert.IsGreaterThan(100_000, new FileInfo(modulePath).Length);
    }
}