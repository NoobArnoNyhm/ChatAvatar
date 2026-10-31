namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed class AvatarRuntimeService
{
    private readonly AvatarCatalog _catalog;
    private readonly IAvatarRenderer _renderer;
    private readonly SemaphoreSlim _selectionGate = new(1, 1);

    public AvatarRuntimeService(AvatarCatalog catalog, IAvatarRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(renderer);
        _catalog = catalog;
        _renderer = renderer;
    }

    public AvatarDefinition? CurrentAvatar { get; private set; }

    public async Task<AvatarDefinition> SelectAsync(
        string directoryPath,
        Guid avatarId,
        CancellationToken cancellationToken = default)
    {
        await _selectionGate.WaitAsync(cancellationToken);
        try
        {
            var avatar = await _catalog.LoadAsync(directoryPath, avatarId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await _renderer.SetAvatarTypeAsync(avatar.Kind);
            CurrentAvatar = avatar;
            return avatar;
        }
        finally
        {
            _selectionGate.Release();
        }
    }
}