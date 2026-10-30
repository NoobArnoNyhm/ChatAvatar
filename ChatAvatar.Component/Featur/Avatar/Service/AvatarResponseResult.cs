namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record AvatarResponseResult(
    Guid ResponseId,
    AvatarResponseStatus Status,
    string? ErrorMessage = null);