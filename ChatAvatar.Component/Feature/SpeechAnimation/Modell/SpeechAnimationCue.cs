namespace ChatAvatar.Component.Feature.SpeechAnimation.Modell;

public sealed record SpeechAnimationCue
{
    public SpeechAnimationCue(
        TimeSpan audioPosition,
        TimeSpan duration,
        SpeechAnimationCueType type,
        string? phoneme = null,
        int? viseme = null,
        string? nextPhoneme = null,
        int? nextViseme = null)
    {
        if (audioPosition < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(audioPosition));
        }

        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        if (type == SpeechAnimationCueType.Phoneme && string.IsNullOrWhiteSpace(phoneme))
        {
            throw new ArgumentException("A phoneme cue requires a phoneme value.", nameof(phoneme));
        }

        if (type == SpeechAnimationCueType.Viseme && (viseme is null or < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(viseme), "A viseme cue requires a non-negative viseme ID.");
        }

        AudioPosition = audioPosition;
        Duration = duration;
        Type = type;
        Phoneme = phoneme;
        Viseme = viseme;
        NextPhoneme = nextPhoneme;
        NextViseme = nextViseme;
    }

    public TimeSpan AudioPosition { get; }

    public TimeSpan Duration { get; }

    public SpeechAnimationCueType Type { get; }

    public string? Phoneme { get; }

    public int? Viseme { get; }

    public string? NextPhoneme { get; }

    public int? NextViseme { get; }
}