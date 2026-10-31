using System.IO;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class SampleAvatarTests
{
    [TestMethod]
    public async Task AllSampleAvatarsLoadFromLocalBuildAssets()
    {
        var sampleDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Avatars");
        var fileStore = new AvatarDefinitionFileStore();
        var catalog = new AvatarCatalog(fileStore);

        var result = await catalog.ListAsync(sampleDirectory);

        Assert.HasCount(3, result.Entries);
        Assert.HasCount(0, result.Issues);
        CollectionAssert.AreEquivalent(
            new[] { AvatarKind.Face, AvatarKind.ImageBased, AvatarKind.Abstract },
            result.Entries.Select(entry => entry.Avatar.Kind).ToArray());
        var face = (FaceAvatarAppearance)result.Entries
            .Single(entry => entry.Avatar.Kind == AvatarKind.Face).Avatar.Appearance;
        Assert.IsNull(face.ModelAssetPath);
        Assert.AreEqual(
            "builtin:image-style-lens",
            ((ImageBasedAvatarAppearance)result.Entries
                .Single(entry => entry.Avatar.Kind == AvatarKind.ImageBased).Avatar.Appearance).TextureAssetPath);
    }

    [TestMethod]
    public async Task EverySampleCanBeActivatedByTheRuntimeService()
    {
        var sampleDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Avatars");
        var fileStore = new AvatarDefinitionFileStore();
        var renderer = new FakeAvatarRenderer();
        var runtime = new AvatarRuntimeService(new AvatarCatalog(fileStore), renderer);
        var samples = await new AvatarCatalog(fileStore).ListAsync(sampleDirectory);

        foreach (var sample in samples.Entries)
        {
            await runtime.SelectAsync(sampleDirectory, sample.Avatar.Id);
        }

        CollectionAssert.AreEquivalent(
            samples.Entries.Select(entry => entry.Avatar.Kind).ToArray(),
            renderer.ActivatedTypes.ToArray());
    }

    private sealed class FakeAvatarRenderer : IAvatarRenderer
    {
        public List<AvatarKind> ActivatedTypes { get; } = [];

        public Task SetAvatarTypeAsync(AvatarKind avatarType)
        {
            ActivatedTypes.Add(avatarType);
            return Task.CompletedTask;
        }
    }
}