namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface ISpeechSynthesisService
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default);
}