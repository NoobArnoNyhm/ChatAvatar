using System.IO;
using ChatAvatar.Component.Feature.SpeechAnimation.Modell;

namespace ChatAvatar.Component.Feature.SpeechSynthesis.Modell;

public sealed record SpeechSynthesisResult(Stream AudioStream, SpeechAnimationTrack AnimationTrack);