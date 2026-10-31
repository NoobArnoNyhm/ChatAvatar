using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class AudioPlaybackSessionTests
{
    [TestMethod]
    public async Task SuccessfulPlaybackReportsStateAndReleasesStream()
    {
        var engine = new FakeAudioPlaybackEngine();
        using var session = new AudioPlaybackSession(engine);
        using var states = new StatusCollector(session);
        var audioStream = new MemoryStream([1, 2, 3]);

        var result = await session.PlayAsync(audioStream);

        Assert.AreEqual(AudioPlaybackStatus.Completed, result.Status);
        Assert.AreEqual(AudioPlaybackStatus.Completed, session.Status);
        CollectionAssert.AreEqual(
            new[] { AudioPlaybackStatus.Playing, AudioPlaybackStatus.Completed },
            states.Statuses);
        Assert.IsFalse(audioStream.CanRead);
        Assert.AreSame(audioStream, engine.ReceivedStream);
    }

    [TestMethod]
    public async Task CancellationReportsCancelledAndReleasesStream()
    {
        var engine = new FakeAudioPlaybackEngine { CancellationRequested = true };
        using var session = new AudioPlaybackSession(engine);
        using var states = new StatusCollector(session);
        var audioStream = new MemoryStream([1, 2, 3]);

        var result = await session.PlayAsync(audioStream);

        Assert.AreEqual(AudioPlaybackStatus.Cancelled, result.Status);
        CollectionAssert.AreEqual(
            new[] { AudioPlaybackStatus.Playing, AudioPlaybackStatus.Cancelled },
            states.Statuses);
        Assert.IsFalse(audioStream.CanRead);
    }

    [TestMethod]
    public async Task CancelStopsAnInProgressEngineTask()
    {
        var engine = new FakeAudioPlaybackEngine { WaitForCancellation = true };
        using var session = new AudioPlaybackSession(engine);
        var audioStream = new MemoryStream([1, 2, 3]);

        var playbackTask = session.PlayAsync(audioStream);
        session.Cancel();
        var result = await playbackTask;

        Assert.AreEqual(AudioPlaybackStatus.Cancelled, result.Status);
        Assert.IsFalse(audioStream.CanRead);
    }

    [TestMethod]
    public async Task PlaybackErrorReportsFailureAndReleasesStream()
    {
        var engine = new FakeAudioPlaybackEngine
        {
            Failure = new InvalidOperationException("Audio device unavailable.")
        };
        using var session = new AudioPlaybackSession(engine);
        using var states = new StatusCollector(session);
        var audioStream = new MemoryStream([1, 2, 3]);

        var result = await session.PlayAsync(audioStream);

        Assert.AreEqual(AudioPlaybackStatus.Failed, result.Status);
        Assert.AreEqual("Audio device unavailable.", result.ErrorMessage);
        CollectionAssert.AreEqual(
            new[] { AudioPlaybackStatus.Playing, AudioPlaybackStatus.Failed },
            states.Statuses);
        Assert.IsFalse(audioStream.CanRead);
    }

    [TestMethod]
    public async Task SessionCannotBeStartedMoreThanOnce()
    {
        using var session = new AudioPlaybackSession(new FakeAudioPlaybackEngine());
        await session.PlayAsync(new MemoryStream([1]));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => session.PlayAsync(new MemoryStream([2])));
    }

    private sealed class FakeAudioPlaybackEngine : IAudioPlaybackEngine
    {
        public Exception? Failure { get; init; }

        public bool CancellationRequested { get; init; }

        public bool WaitForCancellation { get; init; }

        public Stream? ReceivedStream { get; private set; }

        public Task PlayAsync(
            Stream audioStream,
            IProgress<TimeSpan>? positionProgress = null,
            CancellationToken cancellationToken = default)
        {
            ReceivedStream = audioStream;
            if (CancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (Failure is not null)
            {
                throw Failure;
            }

            if (WaitForCancellation)
            {
                return Task.Delay(Timeout.Infinite, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }

    private sealed class StatusCollector : IDisposable
    {
        private readonly AudioPlaybackSession _session;

        public StatusCollector(AudioPlaybackSession session)
        {
            _session = session;
            session.StatusChanged += OnStatusChanged;
        }

        public List<AudioPlaybackStatus> Statuses { get; } = [];

        public void Dispose() => _session.StatusChanged -= OnStatusChanged;

        private void OnStatusChanged(object? sender, AudioPlaybackStatusChangedEventArgs eventArgs) =>
            Statuses.Add(eventArgs.Status);
    }
}