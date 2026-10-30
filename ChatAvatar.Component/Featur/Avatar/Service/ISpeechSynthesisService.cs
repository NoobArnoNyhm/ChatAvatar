namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface ISpeechSynthesisService
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default);
}