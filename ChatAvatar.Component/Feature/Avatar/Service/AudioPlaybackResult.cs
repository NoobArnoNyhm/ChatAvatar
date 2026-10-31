namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed record AudioPlaybackResult(AudioPlaybackStatus Status, string? ErrorMessage = null);