using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SpeechPlaybackCoordinator
{
    private readonly Func<Stream, IAudioLevelAnalyzer> _audioLevelAnalyzerFactory;

    public SpeechPlaybackCoordinator(Func<Stream, IAudioLevelAnalyzer>? audioLevelAnalyzerFactory = null)
    {
        _audioLevelAnalyzerFactory = audioLevelAnalyzerFactory ?? (stream => new WaveAudioLevelAnalyzer(stream));
    }

    public event EventHandler<SpeechPlaybackFrameEventArgs>? FrameChanged;

    public async Task<AudioPlaybackResult> PlayAsync(
        string transcript,
        SpeechSynthesisResult synthesis,
        AudioPlaybackSession playbackSession,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transcript);
        ArgumentNullException.ThrowIfNull(synthesis);
        ArgumentNullException.ThrowIfNull(playbackSession);

        var cursor = new SpeechTimelineCursor(synthesis.AnimationTrack);
        var audioLevelAnalyzer = _audioLevelAnalyzerFactory(synthesis.AudioStream);
        var synchronizationContext = SynchronizationContext.Current;
        var progress = new CallbackProgress(position =>
        {
            var frame = cursor.Advance(transcript, position, audioLevelAnalyzer.GetRmsLevel(position));
            PublishFrame(frame, synchronizationContext);
        });

        try
        {
            return await playbackSession.PlayAsync(
                synthesis.AudioStream,
                progress,
                cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            PublishFrame(SpeechPlaybackFrame.Empty, synchronizationContext);
        }
    }

    private void PublishFrame(SpeechPlaybackFrame frame, SynchronizationContext? synchronizationContext)
    {
        if (synchronizationContext is null || SynchronizationContext.Current == synchronizationContext)
        {
            FrameChanged?.Invoke(this, new SpeechPlaybackFrameEventArgs(frame));
            return;
        }

        synchronizationContext.Send(
            _ => FrameChanged?.Invoke(this, new SpeechPlaybackFrameEventArgs(frame)),
            null);
    }

    private sealed class CallbackProgress(Action<TimeSpan> onProgress) : IProgress<TimeSpan>
    {
        public void Report(TimeSpan value) => onProgress(value);
    }
}