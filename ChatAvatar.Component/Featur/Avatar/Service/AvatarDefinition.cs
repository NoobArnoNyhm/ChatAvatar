namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record AvatarDefinition
{
    public AvatarDefinition(Guid id, string displayName, AvatarAppearance appearance)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("An avatar ID cannot be empty.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(appearance);

        Id = id;
        DisplayName = displayName;
        Appearance = appearance;
    }

    public Guid Id { get; }

    public string DisplayName { get; }

    public AvatarAppearance Appearance { get; }

    public AvatarKind Kind => Appearance.Kind;
}