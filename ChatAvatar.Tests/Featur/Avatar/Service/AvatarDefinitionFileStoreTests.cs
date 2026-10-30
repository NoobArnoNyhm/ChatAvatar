using System.Text.Json;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class AvatarDefinitionFileStoreTests
{
    private readonly AvatarDefinitionFileStore _store = new();

    [TestMethod]
    public async Task SaveAndLoadPreservesEveryAppearanceType()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            AvatarAppearance[] appearances =
            [
                new FaceAvatarAppearance("models/face.glb"),
                new ImageBasedAvatarAppearance("textures/portrait.png"),
                new AbstractAvatarAppearance("#12ABEF", 0.4f)
            ];

            foreach (var appearance in appearances)
            {
                var avatar = new AvatarDefinition(Guid.NewGuid(), "Test Avatar", appearance);
                var expected = new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar);
                var path = Path.Combine(directory, $"{avatar.Id}.avatar.json");

                await _store.SaveAsync(expected, path);
                var actual = await _store.LoadAsync(path);

                Assert.AreEqual(expected, actual);
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadRejectsMalformedJson()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "broken.avatar.json");
        try
        {
            await File.WriteAllTextAsync(path, "{");

            await Assert.ThrowsExactlyAsync<JsonException>(() => _store.LoadAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadRejectsUnknownAppearanceType()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "unknown.avatar.json");
        try
        {
            await File.WriteAllTextAsync(path, """
                {
                  "schemaVersion": 1,
                  "avatar": {
                    "id": "d8e6d6d3-0ca9-4c1c-82db-425e734f27bc",
                    "displayName": "Future Avatar",
                    "appearance": { "kind": "future" }
                  }
                }
                """);

            await Assert.ThrowsExactlyAsync<JsonException>(() => _store.LoadAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadRejectsUnsupportedSchemaVersion()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "future-schema.avatar.json");
        try
        {
            await File.WriteAllTextAsync(path, """
                {
                  "schemaVersion": 2,
                  "avatar": {
                    "id": "d8e6d6d3-0ca9-4c1c-82db-425e734f27bc",
                    "displayName": "Future Avatar",
                    "appearance": { "kind": "face" }
                  }
                }
                """);

            await Assert.ThrowsExactlyAsync<JsonException>(() => _store.LoadAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task LoadUsesOnlyTheExplicitPath()
    {
        var directory = CreateTemporaryDirectory();
        var path = Path.Combine(directory, "missing.avatar.json");
        try
        {
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => _store.LoadAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}