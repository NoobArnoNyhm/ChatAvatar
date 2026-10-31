namespace ChatAvatar.Component.Feature.SpeechAnimation.Modell;

public sealed class SpeechPlaybackFrameEventArgs(SpeechPlaybackFrame frame) : EventArgs
{
    public SpeechPlaybackFrame Frame { get; } = frame;
}