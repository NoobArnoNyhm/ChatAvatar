using System.IO;

namespace ChatAvatar.Component.UI;

public static class WebView2RuntimeLocator
{
    public const string BundledRuntimeDirectoryName = "WebView2FixedRuntime";

    public static string? FindBundledRuntime(string applicationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationDirectory);

        var runtimeDirectory = Path.Combine(applicationDirectory, BundledRuntimeDirectoryName);
        if (!Directory.Exists(runtimeDirectory))
        {
            return null;
        }

        var requiredFiles = new[] { "msedgewebview2.exe", "icudtl.dat", "resources.pak" };
        var missingFiles = requiredFiles
            .Where(file => !File.Exists(Path.Combine(runtimeDirectory, file)))
            .ToArray();
        if (missingFiles.Length > 0 || !Directory.Exists(Path.Combine(runtimeDirectory, "locales")))
        {
            var missing = string.Join(", ", missingFiles);
            if (!Directory.Exists(Path.Combine(runtimeDirectory, "locales")))
            {
                missing = string.IsNullOrEmpty(missing) ? "locales/" : $"{missing}, locales/";
            }

            throw new InvalidDataException($"The bundled WebView2 Fixed Version runtime is incomplete. Missing: {missing}.");
        }

        return runtimeDirectory;
    }
}