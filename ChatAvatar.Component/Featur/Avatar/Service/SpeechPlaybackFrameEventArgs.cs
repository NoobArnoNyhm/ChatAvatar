namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SpeechPlaybackFrameEventArgs(SpeechPlaybackFrame frame) : EventArgs
{
    public SpeechPlaybackFrame Frame { get; } = frame;
}