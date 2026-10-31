namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface IAvatarRenderer
{
    Task SetAvatarTypeAsync(AvatarKind avatarType);
}