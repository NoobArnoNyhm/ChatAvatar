using System.Text.Json;
using System.Text.Json.Serialization;
using ChatAvatar.Component.UI;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class FaceVisemeMessageTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter<AvatarWebMessageType>() }
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
}