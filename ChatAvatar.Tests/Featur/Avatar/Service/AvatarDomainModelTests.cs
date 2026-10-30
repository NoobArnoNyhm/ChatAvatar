using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class AvatarDomainModelTests
{
    [TestMethod]
    public void AvatarDefinitionAcceptsEachSupportedAppearance()
    {
        var face = CreateAvatar(new FaceAvatarAppearance());
        var imageBased = CreateAvatar(new ImageBasedAvatarAppearance("textures/shodan-inspired.png"));
        var abstractAvatar = CreateAvatar(new AbstractAvatarAppearance());

        Assert.AreEqual(AvatarKind.Face, face.Kind);
        Assert.AreEqual(AvatarKind.ImageBased, imageBased.Kind);
        Assert.AreEqual(AvatarKind.Abstract, abstractAvatar.Kind);
    }

    [TestMethod]
    public void AvatarDefinitionRejectsEmptyIdentityAndName()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AvatarDefinition(Guid.Empty, "Assistant", new FaceAvatarAppearance()));
        Assert.ThrowsExactly<ArgumentException>(() => new AvatarDefinition(Guid.NewGuid(), " ", new FaceAvatarAppearance()));
    }

    [TestMethod]
    public void ImageBasedAppearanceRequiresTextureAsset()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ImageBasedAvatarAppearance(" "));
    }

    [TestMethod]
    public void AbstractAppearanceValidatesColorAndPulseRange()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AbstractAvatarAppearance("red"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new AbstractAvatarAppearance(pulseIntensity: 1.1f));
    }

    [TestMethod]
    public void AvatarDocumentUsesCurrentSchemaVersion()
    {
        var document = new AvatarFileDocument(
            AvatarFileDocument.CurrentSchemaVersion,
            CreateAvatar(new FaceAvatarAppearance()));

        Assert.AreEqual(1, document.SchemaVersion);
    }

    [TestMethod]
    public void AvatarDocumentRejectsUnsupportedSchemaVersion()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AvatarFileDocument(2, CreateAvatar(new FaceAvatarAppearance())));
    }

    private static AvatarDefinition CreateAvatar(AvatarAppearance appearance) =>
        new(Guid.NewGuid(), "Test Avatar", appearance);
}