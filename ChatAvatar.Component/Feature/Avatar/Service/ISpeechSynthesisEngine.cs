namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface ISpeechSynthesisEngine
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        string text,
        AvatarResponseSettings settings,
        CancellationToken cancellationToken = default);
}