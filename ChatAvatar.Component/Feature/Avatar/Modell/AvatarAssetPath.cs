using System.IO;
using System.Linq;

namespace ChatAvatar.Component.Feature.Avatar.Modell;

internal static class AvatarAssetPath
{
    public static string Validate(string path, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
        if (path == "builtin:image-style-lens")
        {
            return path;
        }

        var normalized = path.Replace('\\', '/');
        if (Path.IsPathRooted(path)
            || normalized.StartsWith("/", StringComparison.Ordinal)
            || Uri.TryCreate(path, UriKind.Absolute, out _)
            || normalized.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(segment => segment is "." or ".."))
        {
            throw new ArgumentException("Avatar assets must use a local relative path.", parameterName);
        }

        return normalized;
    }

    public static string? ValidateOptional(string? path, string parameterName) =>
        string.IsNullOrWhiteSpace(path) ? null : Validate(path, parameterName);
}