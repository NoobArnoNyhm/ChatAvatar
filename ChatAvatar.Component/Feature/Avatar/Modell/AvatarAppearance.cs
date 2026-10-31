using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Feature.Avatar.Modell;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(FaceAvatarAppearance), "face")]
[JsonDerivedType(typeof(ImageBasedAvatarAppearance), "imageBased")]
[JsonDerivedType(typeof(AbstractAvatarAppearance), "abstract")]
public abstract record AvatarAppearance
{
    private protected AvatarAppearance()
    {
    }

    [JsonIgnore]
    public abstract AvatarKind Kind { get; }
}