using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Feature.Avatar.Modell;

public sealed record FaceAvatarAppearance : AvatarAppearance
{
    public FaceAvatarAppearance(string? modelAssetPath = null, float mouthScale = 1f)
    {
        if (mouthScale is < 0.5f or > 2f)
        {
            throw new ArgumentOutOfRangeException(nameof(mouthScale), "Mouth scale must be between 0.5 and 2.");
        }

        ModelAssetPath = AvatarAssetPath.ValidateOptional(modelAssetPath, nameof(modelAssetPath));
        MouthScale = mouthScale;
    }

    public string? ModelAssetPath { get; }

    public float MouthScale { get; }

    [JsonIgnore]
    public override AvatarKind Kind => AvatarKind.Face;
}