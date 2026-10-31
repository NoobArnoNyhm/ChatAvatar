namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface IAvatarResponseService
{
    AvatarResponseSession Start(AvatarResponseRequest request);
}