using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record SpeechSynthesisResult(Stream AudioStream, SpeechAnimationTrack AnimationTrack);