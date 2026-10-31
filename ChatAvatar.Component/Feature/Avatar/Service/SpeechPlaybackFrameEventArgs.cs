namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed class SpeechPlaybackFrameEventArgs(SpeechPlaybackFrame frame) : EventArgs
{
    public SpeechPlaybackFrame Frame { get; } = frame;
}