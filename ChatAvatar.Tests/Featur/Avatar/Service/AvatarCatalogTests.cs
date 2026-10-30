using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class AvatarCatalogTests
{
    private readonly AvatarDefinitionFileStore _fileStore = new();
    private readonly AvatarCatalog _catalog;

    public AvatarCatalogTests()
    {
        _catalog = new AvatarCatalog(_fileStore);
    }

    [TestMethod]
    public async Task ListReturnsValidAvatarsAndKeepsInvalidFileAsIssue()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var avatar = CreateAvatar("Zeta");
            await SaveAvatarAsync(directory, avatar);
            await File.WriteAllTextAsync(Path.Combine(directory, $"broken{AvatarDefinitionFileStore.FileExtension}"), "{");

            var result = await _catalog.ListAsync(directory);

            Assert.HasCount(1, result.Entries);
            Assert.AreEqual(avatar, result.Entries[0].Avatar);
            Assert.AreEqual(AvatarKind.Face, result.Entries[0].Avatar.Kind);
            Assert.HasCount(1, result.Issues);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ListUsesOnlyTheProvidedDirectory()
    {
        var firstDirectory = CreateTemporaryDirectory();
        var secondDirectory = CreateTemporaryDirectory();
        try
        {
            var firstAvatar = CreateAvatar("First");
            var secondAvatar = CreateAvatar("Second");
            await SaveAvatarAsync(firstDirectory, firstAvatar);
            await SaveAvatarAsync(secondDirectory, secondAvatar);

            var firstResult = await _catalog.ListAsync(firstDirectory);
            var secondResult = await _catalog.ListAsync(secondDirectory);

            Assert.AreEqual(firstAvatar.Id, firstResult.Entries.Single().Avatar.Id);
            Assert.AreEqual(secondAvatar.Id, secondResult.Entries.Single().Avatar.Id);
        }
        finally
        {
            Directory.Delete(firstDirectory, recursive: true);
            Directory.Delete(secondDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadReturnsAvatarByIdentity()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var expected = CreateAvatar("Selected");
            await SaveAvatarAsync(directory, expected);

            var actual = await _catalog.LoadAsync(directory, expected.Id);

            Assert.AreEqual(expected, actual);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadReportsMissingIdentity()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
                () => _catalog.LoadAsync(directory, Guid.NewGuid()));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadReportsDuplicateIdentity()
    {
        var directory = CreateTemporaryDirectory();
        var avatarId = Guid.NewGuid();
        try
        {
            await SaveAvatarAsync(directory, new AvatarDefinition(avatarId, "First", new FaceAvatarAppearance()), "first");
            await SaveAvatarAsync(directory, new AvatarDefinition(avatarId, "Second", new FaceAvatarAppearance()), "second");

            await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => _catalog.LoadAsync(directory, avatarId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private async Task SaveAvatarAsync(string directory, AvatarDefinition avatar, string? fileName = null)
    {
        var document = new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar);
        var path = Path.Combine(directory, $"{fileName ?? avatar.Id.ToString()}{AvatarDefinitionFileStore.FileExtension}");
        await _fileStore.SaveAsync(document, path);
    }

    private static AvatarDefinition CreateAvatar(string name) =>
        new(Guid.NewGuid(), name, new FaceAvatarAppearance());

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarCatalogTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}