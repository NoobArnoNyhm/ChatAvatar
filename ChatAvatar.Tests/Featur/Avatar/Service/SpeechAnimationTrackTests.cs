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
    public void CursorSelectsTextAndNewAnimationCuesAtAudioPosition()
    {
        var phoneme = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(80), SpeechAnimationCueType.Phoneme, phoneme: "h");
        var viseme = new SpeechAnimationCue(
            TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(80), SpeechAnimationCueType.Viseme, viseme: 3);
        var track = new SpeechAnimationTrack(
            [phoneme, viseme],
            [new SpeechTextCue(TimeSpan.FromMilliseconds(100), 0, 5)]);
        var cursor = new SpeechTimelineCursor(track);

        var beforeSpeech = cursor.Advance("Hello world", TimeSpan.FromMilliseconds(50));
        var duringSpeech = cursor.Advance("Hello world", TimeSpan.FromMilliseconds(120));
        var nextFrame = cursor.Advance("Hello world", TimeSpan.FromMilliseconds(150));

        Assert.AreEqual(string.Empty, beforeSpeech.ActiveText);
        Assert.AreEqual("Hello", duringSpeech.ActiveText);
        Assert.AreEqual(0, duringSpeech.CharacterPosition);
        Assert.HasCount(2, duringSpeech.DueAnimationCues);
        Assert.AreEqual(viseme, duringSpeech.ActiveViseme);
        Assert.HasCount(0, nextFrame.DueAnimationCues);
        Assert.AreEqual("Hello world", duringSpeech.Transcript);
    }

    [TestMethod]
    public void CursorRejectsAudioTimeMovingBackwards()
    {
        var cursor = new SpeechTimelineCursor(SpeechAnimationTrack.Empty);
        cursor.Advance("hello", TimeSpan.FromMilliseconds(200));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => cursor.Advance("hello", TimeSpan.FromMilliseconds(100)));
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

        var result = await engine.SynthesizeAsync(
            "Hello avatar",
            new AvatarResponseSettings(voiceName: voices[0]));
        await using var stream = result.AudioStream;
        var audioLevelAnalyzer = new WaveAudioLevelAnalyzer(stream);

        Assert.IsGreaterThan(0, stream.Length);
        Assert.IsGreaterThan(0, audioLevelAnalyzer.Duration.TotalMilliseconds);
        Assert.IsGreaterThan(0, result.AnimationTrack.Cues.Count);
        Assert.IsGreaterThan(0, result.AnimationTrack.TextCues.Count);
        Assert.IsTrue(result.AnimationTrack.Cues.Any(cue => cue.Type == SpeechAnimationCueType.Phoneme));
        Assert.IsTrue(result.AnimationTrack.Cues.Any(cue => cue.Type == SpeechAnimationCueType.Viseme));
        Assert.IsTrue(result.AnimationTrack.Cues.All(cue => cue.AudioPosition >= TimeSpan.Zero));
        Assert.IsTrue(result.AnimationTrack.Cues
            .Zip(result.AnimationTrack.Cues.Skip(1), (current, next) => current.AudioPosition <= next.AudioPosition)
            .All(isOrdered => isOrdered));
        var maximumAudioLevel = 0f;
        for (var position = TimeSpan.Zero; position < audioLevelAnalyzer.Duration; position += TimeSpan.FromMilliseconds(20))
        {
            maximumAudioLevel = Math.Max(maximumAudioLevel, audioLevelAnalyzer.GetRmsLevel(position));
        }

        Assert.IsGreaterThan(0f, maximumAudioLevel);
    }
}