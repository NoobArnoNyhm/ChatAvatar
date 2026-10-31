namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed record AvatarCatalogResult(
    IReadOnlyList<AvatarCatalogEntry> Entries,
    IReadOnlyList<AvatarCatalogIssue> Issues);