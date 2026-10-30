namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface ISpeechSynthesisEngine
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        string text,
        string? voiceName,
        CancellationToken cancellationToken = default);
}