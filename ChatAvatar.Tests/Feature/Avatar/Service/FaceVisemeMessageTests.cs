using System.Text.Json;
using System.Text.Json.Serialization;
using ChatAvatar.Component.UI;
using ChatAvatar.Component.Feature.Avatar.Modell;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class FaceVisemeMessageTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter<AvatarWebMessageType>(),
            new JsonStringEnumConverter<AvatarKind>()
        }
    };

    [TestMethod]
    public void VisemeMessageUsesVersionedTypedPayload()
    {
        var message = new AvatarWebMessage(
            AvatarWebMessageType.SetViseme,
            AvatarWebMessage.CurrentProtocolVersion,
            7);

        var json = JsonSerializer.Serialize(message, SerializerOptions);

        StringAssert.Contains(json, "\"type\":\"SetViseme\"");
        StringAssert.Contains(json, "\"protocolVersion\":1");
        StringAssert.Contains(json, "\"visemeId\":7");
    }

    [TestMethod]
    public void AvatarTypeMessageUsesVersionedTypedPayload()
    {
        var message = new AvatarWebMessage(
            AvatarWebMessageType.SetAvatarType,
            AvatarWebMessage.CurrentProtocolVersion,
            AvatarType: AvatarKind.ImageBased);

        var json = JsonSerializer.Serialize(message, SerializerOptions);

        StringAssert.Contains(json, "\"type\":\"SetAvatarType\"");
        StringAssert.Contains(json, "\"avatarType\":\"ImageBased\"");
    }

    [TestMethod]
    public void AudioLevelMessageUsesVersionedNormalizedPayload()
    {
        var message = new AvatarWebMessage(
            AvatarWebMessageType.SetAudioLevel,
            AvatarWebMessage.CurrentProtocolVersion,
            AudioLevel: 0.5f);

        var json = JsonSerializer.Serialize(message, SerializerOptions);

        StringAssert.Contains(json, "\"type\":\"SetAudioLevel\"");
        StringAssert.Contains(json, "\"audioLevel\":0.5");
    }

    [TestMethod]
    public void AppearanceMessagesContainOnlyPropertiesForTheirAvatarType()
    {
        var face = AvatarWebMessage.ForAppearance(new AvatarDefinition(
            Guid.NewGuid(), "Face", new FaceAvatarAppearance(mouthScale: 1.4f)));
        var image = AvatarWebMessage.ForAppearance(new AvatarDefinition(
            Guid.NewGuid(), "Image", new ImageBasedAvatarAppearance("Avatars/lens.png", "#AABBCC")));
        var abstractAvatar = AvatarWebMessage.ForAppearance(new AvatarDefinition(
            Guid.NewGuid(), "Abstract", new AbstractAvatarAppearance("#123456", 0.3f)));

        Assert.AreEqual(AvatarKind.Face, face.AvatarType);
        Assert.AreEqual(1.4f, face.MouthScale);
        Assert.IsNull(face.TextureAssetPath);
        Assert.AreEqual("#AABBCC", image.TintColor);
        Assert.IsNull(image.MouthScale);
        Assert.AreEqual("#123456", abstractAvatar.LightColor);
        Assert.AreEqual(0.3f, abstractAvatar.PulseIntensity);
    }
}