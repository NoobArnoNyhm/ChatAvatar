using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record FaceAvatarAppearance(string? ModelAssetPath = null) : AvatarAppearance
{
    [JsonIgnore]
    public override AvatarKind Kind => AvatarKind.Face;
}