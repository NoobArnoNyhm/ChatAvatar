namespace ChatAvatar.Component.UI;

public sealed record AvatarWebMessage(AvatarWebMessageType Type, int ProtocolVersion)
{
    public const int CurrentProtocolVersion = 1;
}