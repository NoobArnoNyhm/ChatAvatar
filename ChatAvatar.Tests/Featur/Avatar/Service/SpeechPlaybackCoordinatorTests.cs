using System.IO;
using System.Buffers.Binary;
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
        var audioStream = CreatePcmWave(Enumerable.Repeat(short.MaxValue, 20)
            .Concat(Enumerable.Repeat((short)0, 80))
            .ToArray());
        var synthesis = new SpeechSynthesisResult(audioStream, track);
        var engine = new FakeAudioPlaybackEngine([TimeSpan.Zero, TimeSpan.FromMilliseconds(70)]);
        using var session = new AudioPlaybackSession(engine);
        var coordinator = new SpeechPlaybackCoordinator();
        var frames = new List<SpeechPlaybackFrame>();
        coordinator.FrameChanged += (_, eventArgs) => frames.Add(eventArgs.Frame);

        var result = await coordinator.PlayAsync("Hello world", synthesis, session);

        Assert.AreEqual(AudioPlaybackStatus.Completed, result.Status);
        Assert.HasCount(3, frames);
        Assert.AreEqual(string.Empty, frames[0].ActiveText);
        Assert.AreEqual("Hello", frames[1].ActiveText);
        Assert.IsGreaterThan(0.9f, frames[0].AudioLevel);
        Assert.AreEqual(0, frames[1].AudioLevel);
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
        var synthesis = new SpeechSynthesisResult(CreatePcmWave([0, 0]), SpeechAnimationTrack.Empty);

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

    private static MemoryStream CreatePcmWave(short[] samples)
    {
        var dataLength = samples.Length * 2;
        var bytes = new byte[44 + dataLength];
        "RIFF"u8.CopyTo(bytes);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(4, 4), bytes.Length - 8);
        "WAVEfmt "u8.CopyTo(bytes.AsSpan(8));
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(16, 4), 16);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(20, 2), 1);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(22, 2), 1);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(24, 4), 1000);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(28, 4), 2000);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(32, 2), 2);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(34, 2), 16);
        "data"u8.CopyTo(bytes.AsSpan(36));
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(40, 4), dataLength);
        for (var index = 0; index < samples.Length; index++)
        {
            BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(44 + index * 2, 2), samples[index]);
        }

        return new MemoryStream(bytes);
    }
}