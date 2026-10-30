using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class SpeechSynthesisServiceTests
{
    [TestMethod]
    public async Task ServicePassesTextAndHostVoiceToEngine()
    {
        var engine = new FakeSpeechSynthesisEngine();
        var service = new SpeechSynthesisService(engine);
        var request = new AvatarResponseRequest("Hello", new AvatarResponseSettings("Local Voice"));

        using var stream = await service.SynthesizeAsync(request);

        Assert.AreEqual("Hello", engine.LastText);
        Assert.AreEqual("Local Voice", engine.LastVoiceName);
        Assert.AreEqual(4, stream.Length);
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

        using var stream = await engine.SynthesizeAsync("Test speech", voices[0]);
        var header = new byte[4];
        var bytesRead = await stream.ReadAsync(header);

        Assert.IsGreaterThan(0, stream.Length);
        Assert.AreEqual(4, bytesRead);
        CollectionAssert.AreEqual("RIFF"u8.ToArray(), header);
    }

    private sealed class FakeSpeechSynthesisEngine : ISpeechSynthesisEngine
    {
        public string? LastText { get; private set; }

        public string? LastVoiceName { get; private set; }

        public Exception? Failure { get; init; }

        public Task<Stream> SynthesizeAsync(
            string text,
            string? voiceName,
            CancellationToken cancellationToken = default)
        {
            LastText = text;
            LastVoiceName = voiceName;

            if (Failure is not null)
            {
                return Task.FromException<Stream>(Failure);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<Stream>(new MemoryStream([1, 2, 3, 4]));
        }
    }
}