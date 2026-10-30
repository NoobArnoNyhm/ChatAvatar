namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record AvatarCatalogResult(
    IReadOnlyList<AvatarCatalogEntry> Entries,
    IReadOnlyList<AvatarCatalogIssue> Issues);