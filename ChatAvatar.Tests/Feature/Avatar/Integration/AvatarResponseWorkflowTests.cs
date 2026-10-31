using System.Buffers.Binary;
using System.IO;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.Integration;

[TestClass]
public sealed class AvatarResponseWorkflowTests
{
    [TestMethod]
    public async Task HostRequestSynthesizesSelectsAvatarAndCompletesSynchronizedPlayback()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var avatar = new AvatarDefinition(Guid.NewGuid(), "Workflow Face", new FaceAvatarAppearance());
            var fileStore = new AvatarDefinitionFileStore();
            await fileStore.SaveAsync(
                new AvatarFileDocument(AvatarFileDocument.CurrentSchemaVersion, avatar),
                Path.Combine(directory, $"{avatar.Id}{AvatarDefinitionFileStore.FileExtension}"));

            var renderer = new FakeAvatarRenderer();
            var runtime = new AvatarRuntimeService(new AvatarCatalog(fileStore), renderer);
            var selectedAvatar = await runtime.SelectAsync(directory, avatar.Id);
            var settings = new AvatarResponseSettings("Test Voice", avatar.Id.ToString(), 75, 1);
            var request = new AvatarResponseRequest("Hello workflow", settings);
            var speechEngine = new FakeSpeechEngine();
            var synthesis = await new SpeechSynthesisService(speechEngine).SynthesizeAsync(request);
            var playbackSession = new AudioPlaybackSession(new FakePlaybackEngine([TimeSpan.Zero, TimeSpan.FromMilliseconds(70)]));
            using var ownedPlaybackSession = playbackSession;
            var coordinator = new SpeechPlaybackCoordinator();
            var frames = new List<SpeechPlaybackFrame>();
            coordinator.FrameChanged += (_, eventArgs) => frames.Add(eventArgs.Frame);
            using var response = new AvatarResponseSession();

            Assert.AreEqual(avatar, selectedAvatar);
            Assert.AreSame(settings, speechEngine.ReceivedSettings);
            var playback = await coordinator.PlayAsync(request.Text, synthesis, playbackSession);
            Assert.IsTrue(response.Complete());
            var responseResult = await response.Completion;

            Assert.AreEqual(AudioPlaybackStatus.Completed, playback.Status);
            Assert.AreEqual(AvatarResponseStatus.Completed, responseResult.Status);
            Assert.AreEqual(avatar.Kind, renderer.ActiveType);
            Assert.IsTrue(frames.Any(frame => frame.ActiveText == "Hello"));
            Assert.IsTrue(frames.Any(frame => frame.DueAnimationCues.Any(cue => cue.Type == SpeechAnimationCueType.Viseme)));
            Assert.IsTrue(frames.Any(frame => frame.AudioLevel > 0.9f));
            Assert.AreSame(SpeechPlaybackFrame.Empty, frames[^1]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task HostCancellationTerminatesPlaybackAndResetsTimeline()
    {
        var settings = new AvatarResponseSettings();
        var synthesis = await new SpeechSynthesisService(new FakeSpeechEngine()).SynthesizeAsync(
            new AvatarResponseRequest("Cancel workflow", settings));
        using var playbackSession = new AudioPlaybackSession(
            new FakePlaybackEngine([TimeSpan.Zero, TimeSpan.FromMilliseconds(70)], cancelAfterProgress: true));
        var coordinator = new SpeechPlaybackCoordinator();
        var frames = new List<SpeechPlaybackFrame>();
        coordinator.FrameChanged += (_, eventArgs) => frames.Add(eventArgs.Frame);
        using var response = new AvatarResponseSession();

        var playback = await coordinator.PlayAsync("Cancel workflow", synthesis, playbackSession);
        Assert.IsTrue(response.Cancel());
        var responseResult = await response.Completion;

        Assert.AreEqual(AudioPlaybackStatus.Cancelled, playback.Status);
        Assert.AreEqual(AvatarResponseStatus.Cancelled, responseResult.Status);
        Assert.AreSame(SpeechPlaybackFrame.Empty, frames[^1]);
    }

    [TestMethod]
    public async Task SynthesisFailureCompletesResponseAsFailedWithoutStartingPlayback()
    {
        using var response = new AvatarResponseSession();
        var service = new SpeechSynthesisService(new FakeSpeechEngine
        {
            Failure = new InvalidOperationException("Synthesis failed.")
        });

        var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => service.SynthesizeAsync(new AvatarResponseRequest("Error workflow")));
        Assert.IsTrue(response.Fail(exception.Message));
        var result = await response.Completion;

        Assert.AreEqual(AvatarResponseStatus.Failed, result.Status);
        Assert.AreEqual("Synthesis failed.", result.ErrorMessage);
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ChatAvatarWorkflowTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class FakeAvatarRenderer : IAvatarRenderer
    {
        public AvatarKind? ActiveType { get; private set; }

        public Task SetAvatarTypeAsync(AvatarKind avatarType)
        {
            ActiveType = avatarType;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSpeechEngine : ISpeechSynthesisEngine
    {
        public AvatarResponseSettings? ReceivedSettings { get; private set; }

        public Exception? Failure { get; init; }

        public Task<SpeechSynthesisResult> SynthesizeAsync(
            string text,
            AvatarResponseSettings settings,
            CancellationToken cancellationToken = default)
        {
            ReceivedSettings = settings;
            if (Failure is not null)
            {
                return Task.FromException<SpeechSynthesisResult>(Failure);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var cue = new SpeechAnimationCue(
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromMilliseconds(70),
                SpeechAnimationCueType.Viseme,
                viseme: 2);
            var textCue = new SpeechTextCue(TimeSpan.FromMilliseconds(50), 0, 5);
            return Task.FromResult(new SpeechSynthesisResult(
                CreatePcmWave(Enumerable.Repeat(short.MaxValue, 20)
                    .Concat(Enumerable.Repeat((short)0, 80))
                    .ToArray()),
                new SpeechAnimationTrack([cue], [textCue])));
        }
    }

    private sealed class FakePlaybackEngine(
        IReadOnlyList<TimeSpan> positions,
        bool cancelAfterProgress = false) : IAudioPlaybackEngine
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

            if (cancelAfterProgress)
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