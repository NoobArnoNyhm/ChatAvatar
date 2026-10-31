using ChatAvatar.Component.Feature.AvatarResponse.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Modell;

namespace ChatAvatar.Component.Feature.SpeechSynthesis.Service;

public interface ISpeechSynthesisEngine
{
    Task<SpeechSynthesisResult> SynthesizeAsync(
        string text,
        AvatarResponseSettings settings,
        CancellationToken cancellationToken = default);
}