using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class AvatarDefinitionFileStore
{
    public const string FileExtension = ".avatar.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        WriteIndented = true
    };

    public async Task SaveAsync(
        AvatarFileDocument document,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var fullPath = Path.GetFullPath(filePath);
        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        var temporaryPath = Path.Combine(directory, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, document, SerializerOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public async Task<AvatarFileDocument> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var stream = new FileStream(
            Path.GetFullPath(filePath),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        AvatarFileDocument? document;
        try
        {
            document = await JsonSerializer.DeserializeAsync<AvatarFileDocument>(
                stream,
                SerializerOptions,
                cancellationToken);
        }
        catch (ArgumentException exception)
        {
            throw new JsonException("The avatar file contains invalid document data.", exception);
        }

        return document ?? throw new JsonException("The avatar file did not contain a document.");
    }
}