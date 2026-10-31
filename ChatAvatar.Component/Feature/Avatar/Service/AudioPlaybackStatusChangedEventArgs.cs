namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed class AudioPlaybackStatusChangedEventArgs(AudioPlaybackStatus status) : EventArgs
{
    public AudioPlaybackStatus Status { get; } = status;
}