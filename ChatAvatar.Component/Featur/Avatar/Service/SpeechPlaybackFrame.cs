namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record SpeechPlaybackFrame(
    TimeSpan AudioPosition,
    string Transcript,
    string ActiveText,
    int CharacterPosition,
    int CharacterCount,
    IReadOnlyList<SpeechAnimationCue> DueAnimationCues,
    SpeechAnimationCue? ActiveViseme)
{
    public static SpeechPlaybackFrame Empty { get; } = new(
        TimeSpan.Zero,
        string.Empty,
        string.Empty,
        0,
        0,
        Array.Empty<SpeechAnimationCue>(),
        null);
}