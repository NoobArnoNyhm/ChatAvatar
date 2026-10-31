using System.Linq;

namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed class SpeechTimelineCursor
{
    private readonly SpeechAnimationTrack _track;
    private int _nextAnimationCueIndex;
    private TimeSpan _lastPosition;
    private bool _hasAdvanced;

    public SpeechTimelineCursor(SpeechAnimationTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);
        _track = track;
    }

    public SpeechPlaybackFrame Advance(string transcript, TimeSpan audioPosition, float audioLevel = 0)
    {
        ArgumentNullException.ThrowIfNull(transcript);
        if (audioPosition < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(audioPosition));
        }

        if (!float.IsFinite(audioLevel) || audioLevel is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(audioLevel));
        }

        if (_hasAdvanced && audioPosition < _lastPosition)
        {
            throw new InvalidOperationException("Audio positions must be advanced monotonically.");
        }

        _hasAdvanced = true;
        _lastPosition = audioPosition;

        var textCue = _track.TextCues.LastOrDefault(cue => cue.AudioPosition <= audioPosition);
        var start = Math.Min(textCue?.CharacterPosition ?? 0, transcript.Length);
        var count = Math.Min(textCue?.CharacterCount ?? 0, transcript.Length - start);
        var activeText = count > 0 ? transcript.Substring(start, count) : string.Empty;
        var cueStart = _nextAnimationCueIndex;
        while (_nextAnimationCueIndex < _track.Cues.Count
               && _track.Cues[_nextAnimationCueIndex].AudioPosition <= audioPosition)
        {
            _nextAnimationCueIndex++;
        }

        var dueCues = _track.Cues
            .Skip(cueStart)
            .Take(_nextAnimationCueIndex - cueStart)
            .ToArray();
        var activeViseme = _track.Cues.LastOrDefault(cue =>
            cue.Type == SpeechAnimationCueType.Viseme && cue.AudioPosition <= audioPosition);

        return new SpeechPlaybackFrame(
            audioPosition,
            transcript,
            activeText,
            start,
            count,
            dueCues,
            audioLevel,
            activeViseme);
    }
}