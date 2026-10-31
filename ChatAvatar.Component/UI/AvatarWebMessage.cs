using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Component.UI;

public sealed record AvatarWebMessage(
    AvatarWebMessageType Type,
    int ProtocolVersion,
    int? VisemeId = null,
    AvatarKind? AvatarType = null)
{
    public const int CurrentProtocolVersion = 1;
}