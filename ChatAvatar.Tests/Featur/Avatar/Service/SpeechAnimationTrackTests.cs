using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class SpeechAnimationTrackTests
{
    [TestMethod]
    public void TrackOrdersPhonemeAndVisemeCuesByAudioPosition()
    {
        var later = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(50), SpeechAnimationCueType.Phoneme, phoneme: "a");
        var earlierViseme = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(70), SpeechAnimationCueType.Viseme, viseme: 4);
        var earlierPhoneme = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(70), SpeechAnimationCueType.Phoneme, phoneme: "b");

        var track = new SpeechAnimationTrack([later, earlierViseme, earlierPhoneme]);

        CollectionAssert.AreEqual(new[] { earlierPhoneme, earlierViseme, later }, track.Cues.ToArray());
    }

    [TestMethod]
    public void EmptyTrackContainsNoCues()
    {
        Assert.HasCount(0, SpeechAnimationTrack.Empty.Cues);
    }

    [TestMethod]
    public void CueRejectsInvalidTimeAndMissingTypeValue()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(-1), TimeSpan.Zero, SpeechAnimationCueType.Phoneme, phoneme: "a"));
        Assert.ThrowsExactly<ArgumentException>(() => new SpeechAnimationCue(
            TimeSpan.Zero, TimeSpan.Zero, SpeechAnimationCueType.Phoneme));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SpeechAnimationCue(
            TimeSpan.Zero, TimeSpan.Zero, SpeechAnimationCueType.Viseme, viseme: -1));
    }

    [TestMethod]
    public async Task SystemSpeechProducesOrderedTimedPhonemeAndVisemeCues()
    {
        var engine = new SystemSpeechSynthesisEngine();
        var voices = engine.GetInstalledVoiceNames();
        if (voices.Count == 0)
        {
            Assert.Inconclusive("No local System.Speech voice is installed.");
        }

        var result = await engine.SynthesizeAsync("Hello avatar", voices[0]);
        await using var stream = result.AudioStream;

        Assert.IsGreaterThan(0, stream.Length);
        Assert.IsGreaterThan(0, result.AnimationTrack.Cues.Count);
        Assert.IsTrue(result.AnimationTrack.Cues.Any(cue => cue.Type == SpeechAnimationCueType.Phoneme));
        Assert.IsTrue(result.AnimationTrack.Cues.Any(cue => cue.Type == SpeechAnimationCueType.Viseme));
        Assert.IsTrue(result.AnimationTrack.Cues.All(cue => cue.AudioPosition >= TimeSpan.Zero));
        Assert.IsTrue(result.AnimationTrack.Cues
            .Zip(result.AnimationTrack.Cues.Skip(1), (current, next) => current.AudioPosition <= next.AudioPosition)
            .All(isOrdered => isOrdered));
    }
}