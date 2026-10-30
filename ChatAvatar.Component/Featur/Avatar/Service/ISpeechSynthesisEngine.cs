using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface ISpeechSynthesisEngine
{
    Task<Stream> SynthesizeAsync(
        string text,
        string? voiceName,
        CancellationToken cancellationToken = default);
}