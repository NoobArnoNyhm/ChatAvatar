using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class SpeechPlaybackCoordinatorTests
{
    [TestMethod]
    public async Task CoordinatorPublishesAudioTimedTextAndCuesThenResets()
    {
        var firstCue = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(80), SpeechAnimationCueType.Phoneme, phoneme: "h");
        var viseme = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(80), SpeechAnimationCueType.Viseme, viseme: 2);
        var track = new SpeechAnimationTrack(
            [firstCue, viseme],
            [new SpeechTextCue(TimeSpan.FromMilliseconds(50), 0, 5)]);
        var audioStream = new MemoryStream([1, 2, 3]);
        var synthesis = new SpeechSynthesisResult(audioStream, track);
        var engine = new FakeAudioPlaybackEngine([TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(70)]);
        using var session = new AudioPlaybackSession(engine);
        var coordinator = new SpeechPlaybackCoordinator();
        var frames = new List<SpeechPlaybackFrame>();
        coordinator.FrameChanged += (_, eventArgs) => frames.Add(eventArgs.Frame);

        var result = await coordinator.PlayAsync("Hello world", synthesis, session);

        Assert.AreEqual(AudioPlaybackStatus.Completed, result.Status);
        Assert.HasCount(3, frames);
        Assert.AreEqual(string.Empty, frames[0].ActiveText);
        Assert.AreEqual("Hello", frames[1].ActiveText);
        Assert.HasCount(2, frames[1].DueAnimationCues);
        Assert.AreEqual(viseme, frames[1].ActiveViseme);
        Assert.AreSame(SpeechPlaybackFrame.Empty, frames[2]);
        Assert.IsFalse(audioStream.CanRead);
    }

    [TestMethod]
    public async Task CoordinatorResetsFrameAfterCancellation()
    {
        var engine = new FakeAudioPlaybackEngine([], cancellationRequested: true);
        using var session = new AudioPlaybackSession(engine);
        var coordinator = new SpeechPlaybackCoordinator();
        var frames = new List<SpeechPlaybackFrame>();
        coordinator.FrameChanged += (_, eventArgs) => frames.Add(eventArgs.Frame);
        var synthesis = new SpeechSynthesisResult(new MemoryStream([1]), SpeechAnimationTrack.Empty);

        var result = await coordinator.PlayAsync("Hello", synthesis, session);

        Assert.AreEqual(AudioPlaybackStatus.Cancelled, result.Status);
        Assert.AreSame(SpeechPlaybackFrame.Empty, frames.Single());
    }

    private sealed class FakeAudioPlaybackEngine(
        IReadOnlyList<TimeSpan> positions,
        bool cancellationRequested = false) : IAudioPlaybackEngine
    {
        public Task PlayAsync(
            Stream audioStream,
            IProgress<TimeSpan>? positionProgress = null,
            CancellationToken cancellationToken = default)
        {
            foreach (var position in positions)
            {
                positionProgress?.Report(position);
            }

            if (cancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}