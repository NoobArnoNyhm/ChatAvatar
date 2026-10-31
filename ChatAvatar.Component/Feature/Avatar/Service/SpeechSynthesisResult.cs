using System.IO;

namespace ChatAvatar.Component.Feature.Avatar.Service;

public sealed record SpeechSynthesisResult(Stream AudioStream, SpeechAnimationTrack AnimationTrack);