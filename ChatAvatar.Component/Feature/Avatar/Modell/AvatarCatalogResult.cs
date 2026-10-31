namespace ChatAvatar.Component.Feature.Avatar.Modell;

public sealed record AvatarCatalogResult(
    IReadOnlyList<AvatarCatalogEntry> Entries,
    IReadOnlyList<AvatarCatalogIssue> Issues);