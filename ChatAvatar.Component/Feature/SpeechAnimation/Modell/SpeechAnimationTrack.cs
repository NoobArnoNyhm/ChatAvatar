using System.Linq;

namespace ChatAvatar.Component.Feature.SpeechAnimation.Modell;

public sealed class SpeechAnimationTrack
{
    public SpeechAnimationTrack(
        IEnumerable<SpeechAnimationCue>? cues = null,
        IEnumerable<SpeechTextCue>? textCues = null)
    {
        Cues = (cues ?? [])
            .OrderBy(cue => cue.AudioPosition)
            .ThenBy(cue => cue.Type)
            .ToArray();
            TextCues = (textCues ?? [])
                .OrderBy(cue => cue.AudioPosition)
                .ToArray();
    }

    public IReadOnlyList<SpeechAnimationCue> Cues { get; }

            public IReadOnlyList<SpeechTextCue> TextCues { get; }

    public static SpeechAnimationTrack Empty { get; } = new();
}