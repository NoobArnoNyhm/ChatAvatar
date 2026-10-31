namespace ChatAvatar.Component.UI;

public sealed record AvatarWebMessage(
    AvatarWebMessageType Type,
    int ProtocolVersion,
    int? VisemeId = null)
{
    public const int CurrentProtocolVersion = 1;
}