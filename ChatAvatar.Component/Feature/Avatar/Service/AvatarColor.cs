using System.Text.RegularExpressions;

namespace ChatAvatar.Component.Feature.Avatar.Service;

internal static partial class AvatarColor
{
    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColorPattern();

    public static void Validate(string color, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(color, parameterName);
        if (!HexColorPattern().IsMatch(color))
        {
            throw new ArgumentException("Color must use the #RRGGBB format.", parameterName);
        }
    }
}