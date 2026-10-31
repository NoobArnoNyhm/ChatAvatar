using System.IO;
using ChatAvatar.Component.Feature.AvatarResponse.Modell;
using ChatAvatar.Component.Feature.SpeechAnimation.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Service;

namespace ChatAvatar.Tests.Feature.SpeechSynthesis.Service;

[TestClass]
public sealed class SpeechSynthesisServiceTests
{
    [TestMethod]
    public async Task ServicePassesTextAndHostVoiceToEngine()
    {
        var engine = new FakeSpeechSynthesisEngine();
        var service = new SpeechSynthesisService(engine);
        var settings = new AvatarResponseSettings("Local Voice", volume: 65, rate: 2);
        var request = new AvatarResponseRequest("Hello", settings);

        var result = await service.SynthesizeAsync(request);

        Assert.AreEqual("Hello", engine.LastText);
        Assert.AreSame(settings, engine.LastSettings);
        Assert.AreEqual("Local Voice", engine.LastSettings?.VoiceName);
        Assert.AreEqual(65, engine.LastSettings?.Volume);
        Assert.AreEqual(2, engine.LastSettings?.Rate);
        Assert.AreEqual(4, result.AudioStream.Length);
        Assert.AreSame(SpeechAnimationTrack.Empty, result.AnimationTrack);
        await result.AudioStream.DisposeAsync();
    }

    [TestMethod]
    public async Task ServicePreservesSynthesisErrors()
    {
        var engine = new FakeSpeechSynthesisEngine
        {
            Failure = new InvalidOperationException("Voice unavailable.")
        };
        var service = new SpeechSynthesisService(engine);

        var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => service.SynthesizeAsync(new AvatarResponseRequest("Hello")));

        Assert.AreEqual("Voice unavailable.", exception.Message);
    }

    [TestMethod]
    public async Task ServiceDoesNotCallEngineForCancelledRequest()
    {
        var engine = new FakeSpeechSynthesisEngine();
        var service = new SpeechSynthesisService(engine);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => service.SynthesizeAsync(new AvatarResponseRequest("Hello"), cancellationSource.Token));

        Assert.IsNull(engine.LastText);
    }

    [TestMethod]
    public async Task SystemSpeechProducesWaveDataWhenLocalVoiceIsInstalled()
    {
        var engine = new SystemSpeechSynthesisEngine();
        var voices = engine.GetInstalledVoiceNames();
        if (voices.Count == 0)
        {
            Assert.Inconclusive("No local System.Speech voice is installed.");
        }

        var result = await engine.SynthesizeAsync("Test speech", new AvatarResponseSettings(voiceName: voices[0]));
        await using var stream = result.AudioStream;
        var header = new byte[4];
        var bytesRead = await stream.ReadAsync(header);

        Assert.IsGreaterThan(0, stream.Length);
        Assert.AreEqual(4, bytesRead);
        CollectionAssert.AreEqual("RIFF"u8.ToArray(), header);
    }

    private sealed class FakeSpeechSynthesisEngine : ISpeechSynthesisEngine
    {
        public string? LastText { get; private set; }

        public AvatarResponseSettings? LastSettings { get; private set; }

        public Exception? Failure { get; init; }

        public Task<SpeechSynthesisResult> SynthesizeAsync(
            string text,
            AvatarResponseSettings settings,
            CancellationToken cancellationToken = default)
        {
            LastText = text;
            LastSettings = settings;

            if (Failure is not null)
            {
                return Task.FromException<SpeechSynthesisResult>(Failure);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new SpeechSynthesisResult(
                new MemoryStream([1, 2, 3, 4]),
                SpeechAnimationTrack.Empty));
        }
    }
}