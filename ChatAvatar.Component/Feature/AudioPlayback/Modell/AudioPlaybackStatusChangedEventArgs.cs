namespace ChatAvatar.Component.Feature.AudioPlayback.Modell;

public sealed class AudioPlaybackStatusChangedEventArgs(AudioPlaybackStatus status) : EventArgs
{
    public AudioPlaybackStatus Status { get; } = status;
}