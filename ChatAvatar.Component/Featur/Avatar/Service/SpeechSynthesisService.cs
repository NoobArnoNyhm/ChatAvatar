using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SpeechSynthesisService : ISpeechSynthesisService
{
    private readonly ISpeechSynthesisEngine _engine;

    public SpeechSynthesisService(ISpeechSynthesisEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _engine = engine;
    }

    public Task<Stream> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return _engine.SynthesizeAsync(
            request.Text,
            request.Settings.VoiceName,
            cancellationToken);
    }
}