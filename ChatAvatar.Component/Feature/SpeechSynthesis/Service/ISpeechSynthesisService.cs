using ChatAvatar.Component.Feature.AvatarResponse.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Modell;

namespace ChatAvatar.Component.Feature.SpeechSynthesis.Service;

public interface ISpeechSynthesisService
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default);
}