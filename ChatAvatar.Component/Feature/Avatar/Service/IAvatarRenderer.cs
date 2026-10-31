namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface IAvatarRenderer
{
    Task SetAvatarTypeAsync(AvatarKind avatarType);
}