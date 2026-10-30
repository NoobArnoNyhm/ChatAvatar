using System.IO;
using System.Media;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SystemSoundPlayerEngine : IAudioPlaybackEngine
{
    public async Task PlayAsync(Stream audioStream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioStream);
        if (!audioStream.CanRead || !audioStream.CanSeek)
        {
            throw new ArgumentException("Audio playback requires a readable, seekable WAV stream.", nameof(audioStream));
        }

        cancellationToken.ThrowIfCancellationRequested();
        audioStream.Position = 0;

        using var player = new SoundPlayer(audioStream);
        player.Load();
        using var cancellationRegistration = cancellationToken.Register(player.Stop);
        await Task.Run(player.PlaySync, CancellationToken.None).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
    }
}