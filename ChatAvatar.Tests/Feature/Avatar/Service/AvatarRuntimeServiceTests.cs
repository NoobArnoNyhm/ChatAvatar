using System.IO;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class AvatarRuntimeServiceTests
{
    [TestMethod]
    public async Task SelectionLoadsAndActivatesEverySupportedAvatarType()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var avatars = new[]
            {
                CreateAvatar("Face", new FaceAvatarAppearance()),
                CreateAvatar("Image", new ImageBasedAvatarAppearance("textures/lens.png")),
                CreateAvatar("Abstract", new AbstractAvatarAppearance())
            };
            var store = new AvatarDefinitionFileStore();
            foreach (var avatar in avatars)
            {
                var path = Path.Combine(directory, $"{avatar.Id}{AvatarDefinitionFileStore.FileExtension}");
                await store.SaveAsync(new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar), path);
            }

            var renderer = new FakeAvatarRenderer();
            var service = new AvatarRuntimeService(new AvatarCatalog(store), renderer);
            foreach (var avatar in avatars)
            {
                var selected = await service.SelectAsync(directory, avatar.Id);

                Assert.AreEqual(avatar, selected);
                Assert.AreEqual(avatar, service.CurrentAvatar);
                Assert.AreEqual(avatar.Kind, renderer.ActiveType);
            }

            CollectionAssert.AreEqual(
                avatars.Select(avatar => avatar.Kind).ToArray(),
                renderer.ActivatedTypes.ToArray());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task MissingAvatarDoesNotChangeCurrentSelection()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var current = CreateAvatar("Current", new FaceAvatarAppearance());
            var store = new AvatarDefinitionFileStore();
            var path = Path.Combine(directory, $"{current.Id}{AvatarDefinitionFileStore.FileExtension}");
            await store.SaveAsync(new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, current), path);
            var renderer = new FakeAvatarRenderer();
            var service = new AvatarRuntimeService(new AvatarCatalog(store), renderer);
            await service.SelectAsync(directory, current.Id);

            await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
                () => service.SelectAsync(directory, Guid.NewGuid()));

            Assert.AreEqual(current, service.CurrentAvatar);
            Assert.HasCount(1, renderer.ActivatedTypes);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task RendererFailureDoesNotCommitNewSelection()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var avatar = CreateAvatar("Failed activation", new AbstractAvatarAppearance());
            var store = new AvatarDefinitionFileStore();
            var path = Path.Combine(directory, $"{avatar.Id}{AvatarDefinitionFileStore.FileExtension}");
            await store.SaveAsync(new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar), path);
            var renderer = new FakeAvatarRenderer { Failure = new InvalidOperationException("Renderer unavailable.") };
            var service = new AvatarRuntimeService(new AvatarCatalog(store), renderer);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.SelectAsync(directory, avatar.Id));

            Assert.IsNull(service.CurrentAvatar);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static AvatarDefinition CreateAvatar(string name, AvatarAppearance appearance) =>
        new(Guid.NewGuid(), name, appearance);

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarRuntimeTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class FakeAvatarRenderer : IAvatarRenderer
    {
        public Exception? Failure { get; init; }

        public AvatarKind? ActiveType { get; private set; }

        public List<AvatarKind> ActivatedTypes { get; } = [];

        public Task SetAvatarTypeAsync(AvatarKind avatarType)
        {
            if (Failure is not null)
            {
                return Task.FromException(Failure);
            }

            ActiveType = avatarType;
            ActivatedTypes.Add(avatarType);
            return Task.CompletedTask;
        }
    }
}