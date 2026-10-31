using System.IO;
using ChatAvatar.Component.Feature.AudioPlayback.Modell;

namespace ChatAvatar.Component.Feature.AudioPlayback.Service;

public sealed class AudioPlaybackSession : IDisposable
{
    private readonly IAudioPlaybackEngine _engine;
    private readonly CancellationTokenSource _cancellationSource = new();
    private int _status = (int)AudioPlaybackStatus.Created;
    private int _hasStarted;

    public AudioPlaybackSession(IAudioPlaybackEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _engine = engine;
    }

    public event EventHandler<AudioPlaybackStatusChangedEventArgs>? StatusChanged;

    public AudioPlaybackStatus Status => (AudioPlaybackStatus)Volatile.Read(ref _status);

    public async Task<AudioPlaybackResult> PlayAsync(
        Stream audioStream,
        IProgress<TimeSpan>? positionProgress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioStream);
        if (Interlocked.Exchange(ref _hasStarted, 1) != 0)
        {
            throw new InvalidOperationException("An audio playback session can only be started once.");
        }

        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _cancellationSource.Token);
        SetStatus(AudioPlaybackStatus.Playing);

        try
        {
            linkedCancellation.Token.ThrowIfCancellationRequested();
            await _engine.PlayAsync(audioStream, positionProgress, linkedCancellation.Token).ConfigureAwait(false);
            SetStatus(AudioPlaybackStatus.Completed);
            return new AudioPlaybackResult(AudioPlaybackStatus.Completed);
        }
        catch (OperationCanceledException)
        {
            SetStatus(AudioPlaybackStatus.Cancelled);
            return new AudioPlaybackResult(AudioPlaybackStatus.Cancelled);
        }
        catch (Exception exception)
        {
            SetStatus(AudioPlaybackStatus.Failed);
            return new AudioPlaybackResult(AudioPlaybackStatus.Failed, exception.Message);
        }
        finally
        {
            await audioStream.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Cancel()
    {
        if (Status == AudioPlaybackStatus.Playing)
        {
            _cancellationSource.Cancel();
        }
    }

    public void Dispose()
    {
        _cancellationSource.Cancel();
        _cancellationSource.Dispose();
    }

    private void SetStatus(AudioPlaybackStatus status)
    {
        Volatile.Write(ref _status, (int)status);
        StatusChanged?.Invoke(this, new AudioPlaybackStatusChangedEventArgs(status));
    }
}