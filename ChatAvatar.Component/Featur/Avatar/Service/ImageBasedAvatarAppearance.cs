using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record ImageBasedAvatarAppearance : AvatarAppearance
{
    public ImageBasedAvatarAppearance(string textureAssetPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textureAssetPath);

        TextureAssetPath = textureAssetPath;
    }

    public string TextureAssetPath { get; }

    [JsonIgnore]
    public override AvatarKind Kind => AvatarKind.ImageBased;
}