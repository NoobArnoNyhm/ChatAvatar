using System.IO;
using System.Media;
using System.Diagnostics;
using System.Threading;

namespace ChatAvatar.Component.Feature.AudioPlayback.Service;

public sealed class SystemSoundPlayerEngine : IAudioPlaybackEngine
{
    public async Task PlayAsync(
        Stream audioStream,
        IProgress<TimeSpan>? positionProgress = null,
        CancellationToken cancellationToken = default)
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
        if (positionProgress is null)
        {
            await Task.Run(player.PlaySync, CancellationToken.None).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return;
        }

        using var progressCancellation = new CancellationTokenSource();
        var stopwatch = Stopwatch.StartNew();
        var progressTask = ReportPositionAsync(stopwatch, positionProgress, progressCancellation.Token);
        try
        {
            await Task.Run(player.PlaySync, CancellationToken.None).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            progressCancellation.Cancel();
            try
            {
                await progressTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (progressCancellation.IsCancellationRequested)
            {
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    private static async Task ReportPositionAsync(
        Stopwatch stopwatch,
        IProgress<TimeSpan> positionProgress,
        CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(20));
        positionProgress.Report(TimeSpan.Zero);
        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            positionProgress.Report(stopwatch.Elapsed);
        }
    }
}