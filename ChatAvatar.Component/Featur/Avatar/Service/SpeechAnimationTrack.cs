using System.Linq;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SpeechAnimationTrack
{
    public SpeechAnimationTrack(IEnumerable<SpeechAnimationCue>? cues = null)
    {
        Cues = (cues ?? [])
            .OrderBy(cue => cue.AudioPosition)
            .ThenBy(cue => cue.Type)
            .ToArray();
    }

    public IReadOnlyList<SpeechAnimationCue> Cues { get; }

    public static SpeechAnimationTrack Empty { get; } = new();
}