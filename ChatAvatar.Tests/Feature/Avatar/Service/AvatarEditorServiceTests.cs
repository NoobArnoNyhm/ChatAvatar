using System.IO;
using ChatAvatar.Component.Feature.Avatar.Modell;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class AvatarEditorServiceTests
{
    [TestMethod]
    public void CreateNewProvidesDefaultsForAllAvatarTypes()
    {
        var service = CreateService();

        foreach (var kind in Enum.GetValues<AvatarKind>())
        {
            var avatar = service.CreateNew("New avatar", kind);

            Assert.AreEqual(kind, avatar.Kind);
            Assert.AreNotEqual(Guid.Empty, avatar.Id);
        }
    }

    [TestMethod]
    public async Task SaveUpdatesExistingAvatarFileAndListCanOpenIt()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var service = CreateService();
            var avatar = service.CreateNew("Before", AvatarKind.Face);
            await service.SaveAsync(directory, avatar);
            var updated = new AvatarDefinition(avatar.Id, "After", avatar.Appearance);

            await service.SaveAsync(directory, updated);
            var list = await service.ListAsync(directory);
            var opened = await service.OpenAsync(directory, avatar.Id);

            Assert.HasCount(1, list.Entries);
            Assert.AreEqual("After", opened.DisplayName);
            Assert.AreEqual(avatar.Id, opened.Id);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void CreateNewRejectsUnsupportedTypeAndInvalidName()
    {
        var service = CreateService();

        Assert.ThrowsExactly<ArgumentException>(() => service.CreateNew(" ", AvatarKind.Face));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.CreateNew("Valid", (AvatarKind)99));
    }

    private static AvatarEditorService CreateService()
    {
        var fileStore = new AvatarDefinitionFileStore();
        return new AvatarEditorService(new AvatarCatalog(fileStore), fileStore);
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarEditorTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}