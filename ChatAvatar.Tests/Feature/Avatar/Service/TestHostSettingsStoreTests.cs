using System.IO;
using ChatAvatar.TestApp;
using ChatAvatar.Component.Feature.AvatarResponse.Modell;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class TestHostSettingsStoreTests
{
    [TestMethod]
    public async Task MissingSettingsFileUsesDefaultsWithoutCreatingStorage()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "settings.json");
        try
        {
            var settings = await new TestHostSettingsStore(path).LoadAsync();

            Assert.AreEqual(new AvatarResponseSettings(), settings);
            Assert.IsFalse(File.Exists(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task HostCanPersistAndReloadItsSettings()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "settings.json");
        try
        {
            var store = new TestHostSettingsStore(path);
            var expected = new AvatarResponseSettings("Local Voice", "avatar-id", 68, 3);

            await store.SaveAsync(expected);
            var actual = await store.LoadAsync();

            Assert.AreEqual(expected, actual);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarHostSettingsTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}