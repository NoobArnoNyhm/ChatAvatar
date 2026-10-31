using System.Text.Json;
using System.Text.Json.Serialization;
using ChatAvatar.Component.Featur.Avatar.Service;
using ChatAvatar.Component.UI;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

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
}