using System.IO;
using System.Linq;
using System.Text.Json;

namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed class AvatarCatalog
{
    private readonly AvatarDefinitionFileStore _fileStore;

    public AvatarCatalog(AvatarDefinitionFileStore fileStore)
    {
        ArgumentNullException.ThrowIfNull(fileStore);
        _fileStore = fileStore;
    }

    public async Task<AvatarCatalogResult> ListAsync(
        string directoryPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        var rootPath = Path.GetFullPath(directoryPath);
        var entries = new List<AvatarCatalogEntry>();
        var issues = new List<AvatarCatalogIssue>();

        foreach (var filePath in Directory.EnumerateFiles(
                     rootPath,
                     $"*{AvatarDefinitionFileStore.FileExtension}",
                     SearchOption.TopDirectoryOnly)
                 .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var document = await _fileStore.LoadAsync(filePath, cancellationToken);
                entries.Add(new AvatarCatalogEntry(document.Avatar, filePath));
            }
            catch (Exception exception) when (exception is JsonException or IOException or UnauthorizedAccessException)
            {
                issues.Add(new AvatarCatalogIssue(filePath, exception.Message));
            }
        }

        var orderedEntries = entries
            .OrderBy(entry => entry.Avatar.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.Avatar.Id)
            .ToList()
            .AsReadOnly();

        return new AvatarCatalogResult(orderedEntries, issues.AsReadOnly());
    }

    public async Task<AvatarDefinition> LoadAsync(
        string directoryPath,
        Guid avatarId,
        CancellationToken cancellationToken = default)
    {
        if (avatarId == Guid.Empty)
        {
            throw new ArgumentException("An avatar ID cannot be empty.", nameof(avatarId));
        }

        var catalog = await ListAsync(directoryPath, cancellationToken);
        var matches = catalog.Entries.Where(entry => entry.Avatar.Id == avatarId).ToArray();

        return matches.Length switch
        {
            1 => matches[0].Avatar,
            0 => throw new KeyNotFoundException($"Avatar '{avatarId}' was not found in '{directoryPath}'."),
            _ => throw new InvalidDataException($"Avatar ID '{avatarId}' occurs in multiple files in '{directoryPath}'.")
        };
    }
}