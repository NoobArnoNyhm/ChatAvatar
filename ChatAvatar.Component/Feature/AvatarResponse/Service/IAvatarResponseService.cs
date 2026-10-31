using ChatAvatar.Component.Feature.AvatarResponse.Modell;

namespace ChatAvatar.Component.Feature.AvatarResponse.Service;

public interface IAvatarResponseService
{
    AvatarResponseSession Start(AvatarResponseRequest request);
}