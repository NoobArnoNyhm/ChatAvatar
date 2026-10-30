using System.Text.RegularExpressions;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record AbstractAvatarAppearance : AvatarAppearance
{
    private static readonly Regex HexColorPattern = new("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public AbstractAvatarAppearance(string lightColor = "#FFFFFF", float pulseIntensity = 0.65f)
    {
        if (!HexColorPattern.IsMatch(lightColor))
        {
            throw new ArgumentException("Light color must use the #RRGGBB format.", nameof(lightColor));
        }

        if (pulseIntensity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pulseIntensity), "Pulse intensity must be between 0 and 1.");
        }

        LightColor = lightColor;
        PulseIntensity = pulseIntensity;
    }

    public string LightColor { get; }

    public float PulseIntensity { get; }

    public override AvatarKind Kind => AvatarKind.Abstract;
}