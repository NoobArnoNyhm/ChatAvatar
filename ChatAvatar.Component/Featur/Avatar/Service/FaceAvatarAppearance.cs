namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record FaceAvatarAppearance(string? ModelAssetPath = null) : AvatarAppearance
{
    public override AvatarKind Kind => AvatarKind.Face;
}