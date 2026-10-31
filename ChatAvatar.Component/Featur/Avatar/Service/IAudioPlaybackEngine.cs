using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface IAudioPlaybackEngine
{
    Task PlayAsync(
        Stream audioStream,
        IProgress<TimeSpan>? positionProgress = null,
        CancellationToken cancellationToken = default);
}