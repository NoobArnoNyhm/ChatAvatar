namespace ChatAvatar.Component.Featur.Avatar.Service;

public abstract record AvatarAppearance
{
    private protected AvatarAppearance()
    {
    }

    public abstract AvatarKind Kind { get; }
}