namespace ChatAvatar.Component.Feature.AvatarResponse.Modell;

public sealed record AvatarResponseResult(
    Guid ResponseId,
    AvatarResponseStatus Status,
    string? ErrorMessage = null);