using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface ISpeechSynthesisService
{
    Task<Stream> SynthesizeAsync(
        AvatarResponseRequest request,
        CancellationToken cancellationToken = default);
}