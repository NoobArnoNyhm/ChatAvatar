using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Feature.Avatar.Modell;

public sealed record ImageBasedAvatarAppearance : AvatarAppearance
{
    public ImageBasedAvatarAppearance(string textureAssetPath, string tintColor = "#E2A56F")
    {
        textureAssetPath = AvatarAssetPath.Validate(textureAssetPath, nameof(textureAssetPath));
        AvatarColor.Validate(tintColor, nameof(tintColor));

        TextureAssetPath = textureAssetPath;
        TintColor = tintColor;
    }

    public string TextureAssetPath { get; }

    public string TintColor { get; }

    [JsonIgnore]
    public override AvatarKind Kind => AvatarKind.ImageBased;
}