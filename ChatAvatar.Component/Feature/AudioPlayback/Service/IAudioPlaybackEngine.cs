using System.IO;

namespace ChatAvatar.Component.Feature.AudioPlayback.Service;

public interface IAudioPlaybackEngine
{
    Task PlayAsync(
        Stream audioStream,
        IProgress<TimeSpan>? positionProgress = null,
        CancellationToken cancellationToken = default);
}