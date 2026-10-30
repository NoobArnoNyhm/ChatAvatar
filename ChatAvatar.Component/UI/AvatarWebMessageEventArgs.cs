namespace ChatAvatar.Component.UI;

public sealed class AvatarWebMessageEventArgs(AvatarWebMessage message) : EventArgs
{
    public AvatarWebMessage Message { get; } = message;
}