using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed record AbstractAvatarAppearance : AvatarAppearance
{
    public AbstractAvatarAppearance(string lightColor = "#FFFFFF", float pulseIntensity = 0.65f)
    {
        AvatarColor.Validate(lightColor, nameof(lightColor));

        if (pulseIntensity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pulseIntensity), "Pulse intensity must be between 0 and 1.");
        }

        LightColor = lightColor;
        PulseIntensity = pulseIntensity;
    }

    public string LightColor { get; }

    public float PulseIntensity { get; }

    [JsonIgnore]
    public override AvatarKind Kind => AvatarKind.Abstract;
}