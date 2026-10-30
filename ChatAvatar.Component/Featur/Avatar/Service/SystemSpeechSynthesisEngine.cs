using System.IO;
using System.Speech.Synthesis;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class SystemSpeechSynthesisEngine : ISpeechSynthesisEngine
{
    public IReadOnlyList<string> GetInstalledVoiceNames()
    {
        using var synthesizer = new SpeechSynthesizer();
        return synthesizer.GetInstalledVoices()
            .Where(voice => voice.Enabled)
            .Select(voice => voice.VoiceInfo.Name)
            .ToArray();
    }

    public async Task<Stream> SynthesizeAsync(
        string text,
        string? voiceName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        cancellationToken.ThrowIfCancellationRequested();

        var synthesizer = new SpeechSynthesizer();
        var output = new MemoryStream();
        var completion = new TaskCompletionSource<SpeakCompletedEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler<SpeakCompletedEventArgs> completedHandler = (_, eventArgs) => completion.TrySetResult(eventArgs);
        synthesizer.SpeakCompleted += completedHandler;

        try
        {
            if (!string.IsNullOrWhiteSpace(voiceName))
            {
                synthesizer.SelectVoice(voiceName);
            }

            synthesizer.SetOutputToWaveStream(output);
            using var cancellationRegistration = cancellationToken.Register(synthesizer.SpeakAsyncCancelAll);
            cancellationToken.ThrowIfCancellationRequested();
            synthesizer.SpeakAsync(text);

            var result = await completion.Task.ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested || result.Cancelled)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (result.Error is not null)
            {
                throw new InvalidOperationException("System.Speech could not synthesize the requested text.", result.Error);
            }

            output.Position = 0;
            return output;
        }
        catch
        {
            output.Dispose();
            throw;
        }
        finally
        {
            synthesizer.SpeakCompleted -= completedHandler;
            synthesizer.Dispose();
        }
    }
}