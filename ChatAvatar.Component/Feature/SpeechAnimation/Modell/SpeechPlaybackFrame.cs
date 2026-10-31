namespace ChatAvatar.Component.Feature.SpeechAnimation.Modell;

public sealed record SpeechPlaybackFrame(
    TimeSpan AudioPosition,
    string Transcript,
    string ActiveText,
    int CharacterPosition,
    int CharacterCount,
    IReadOnlyList<SpeechAnimationCue> DueAnimationCues,
    float AudioLevel,
    SpeechAnimationCue? ActiveViseme)
{
    public static SpeechPlaybackFrame Empty { get; } = new(
        TimeSpan.Zero,
        string.Empty,
        string.Empty,
        0,
        0,
        Array.Empty<SpeechAnimationCue>(),
        0,
        null);
}