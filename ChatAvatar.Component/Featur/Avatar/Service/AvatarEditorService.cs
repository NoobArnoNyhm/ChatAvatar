using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class AvatarEditorService
{
    private readonly AvatarCatalog _catalog;
    private readonly AvatarDefinitionFileStore _fileStore;

    public AvatarEditorService(AvatarCatalog catalog, AvatarDefinitionFileStore fileStore)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(fileStore);
        _catalog = catalog;
        _fileStore = fileStore;
    }

    public Task<AvatarCatalogResult> ListAsync(
        string directoryPath,
        CancellationToken cancellationToken = default) =>
        _catalog.ListAsync(directoryPath, cancellationToken);

    public Task<AvatarDefinition> OpenAsync(
        string directoryPath,
        Guid avatarId,
        CancellationToken cancellationToken = default) =>
        _catalog.LoadAsync(directoryPath, avatarId, cancellationToken);

    public AvatarDefinition CreateNew(string displayName, AvatarKind kind) =>
        new(Guid.NewGuid(), displayName, CreateDefaultAppearance(kind));

    public async Task SaveAsync(
        string directoryPath,
        AvatarDefinition avatar,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentNullException.ThrowIfNull(avatar);

        var path = Path.Combine(
            Path.GetFullPath(directoryPath),
            $"{avatar.Id}{AvatarDefinitionFileStore.FileExtension}");
        await _fileStore.SaveAsync(
            new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar),
            path,
            cancellationToken);
    }

    public static AvatarAppearance CreateDefaultAppearance(AvatarKind kind) => kind switch
    {
        AvatarKind.Face => new FaceAvatarAppearance(),
        AvatarKind.ImageBased => new ImageBasedAvatarAppearance("builtin:image-style-lens"),
        AvatarKind.Abstract => new AbstractAvatarAppearance(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}