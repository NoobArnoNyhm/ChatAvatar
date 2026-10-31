using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Component.UI;

public sealed record AvatarWebMessage(
    AvatarWebMessageType Type,
    int ProtocolVersion,
    int? VisemeId = null,
    AvatarKind? AvatarType = null,
    float? AudioLevel = null,
    float? MouthScale = null,
    string? TextureAssetPath = null,
    string? TintColor = null,
    string? LightColor = null,
    float? PulseIntensity = null,
    string? ErrorMessage = null,
    int? CanvasWidth = null,
    int? CanvasHeight = null)
{
    public const int CurrentProtocolVersion = 1;

    public static AvatarWebMessage ForAppearance(AvatarDefinition avatar)
    {
        ArgumentNullException.ThrowIfNull(avatar);

        return avatar.Appearance switch
        {
            FaceAvatarAppearance face => new AvatarWebMessage(
                AvatarWebMessageType.SetAvatarAppearance,
                CurrentProtocolVersion,
                AvatarType: AvatarKind.Face,
                MouthScale: face.MouthScale),
            ImageBasedAvatarAppearance image => new AvatarWebMessage(
                AvatarWebMessageType.SetAvatarAppearance,
                CurrentProtocolVersion,
                AvatarType: AvatarKind.ImageBased,
                TextureAssetPath: image.TextureAssetPath,
                TintColor: image.TintColor),
            AbstractAvatarAppearance abstractAvatar => new AvatarWebMessage(
                AvatarWebMessageType.SetAvatarAppearance,
                CurrentProtocolVersion,
                AvatarType: AvatarKind.Abstract,
                LightColor: abstractAvatar.LightColor,
                PulseIntensity: abstractAvatar.PulseIntensity),
            _ => throw new ArgumentOutOfRangeException(nameof(avatar), "The avatar appearance type is not supported.")
        };
    }
}