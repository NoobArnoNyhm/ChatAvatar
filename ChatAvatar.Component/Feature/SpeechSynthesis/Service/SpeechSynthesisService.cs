using ChatAvatar.Component.Feature.AvatarResponse.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Modell;

namespace ChatAvatar.Component.Feature.SpeechSynthesis.Service;

public sealed class SpeechSynthesisService : ISpeechSynthesisService
{
    private readonly ISpeechSynthesisEngine _engine;

    public SpeechSynthesisService(ISpeechSynthesisEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _engine = engine;
    }

    public Task<SpeechSynthesisResult> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return _engine.SynthesizeAsync(
            request.Text,
            request.Settings,
            cancellationToken);
    }
}