using System.IO;

namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface IAudioPlaybackEngine
{
    Task PlayAsync(
        Stream audioStream,
        IProgress<TimeSpan>? positionProgress = null,
        CancellationToken cancellationToken = default);
}