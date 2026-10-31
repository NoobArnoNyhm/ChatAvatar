namespace ChatAvatar.Component.Feature.SpeechAnimation.Modell;

public sealed record SpeechTextCue
{
    public SpeechTextCue(TimeSpan audioPosition, int characterPosition, int characterCount)
    {
        if (audioPosition < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(audioPosition));
        }

        if (characterPosition < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(characterPosition));
        }

        if (characterCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(characterCount));
        }

        AudioPosition = audioPosition;
        CharacterPosition = characterPosition;
        CharacterCount = characterCount;
    }

    public TimeSpan AudioPosition { get; }

    public int CharacterPosition { get; }

    public int CharacterCount { get; }
}