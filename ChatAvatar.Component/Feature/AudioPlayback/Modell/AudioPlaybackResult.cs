namespace ChatAvatar.Component.Feature.AudioPlayback.Modell;

public sealed record AudioPlaybackResult(AudioPlaybackStatus Status, string? ErrorMessage = null);