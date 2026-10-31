using System.IO;
using System.Collections.Concurrent;
using System.Speech.Synthesis;
using ChatAvatar.Component.Feature.AvatarResponse.Modell;
using ChatAvatar.Component.Feature.SpeechAnimation.Modell;
using ChatAvatar.Component.Feature.SpeechSynthesis.Modell;

namespace ChatAvatar.Component.Feature.SpeechSynthesis.Service;

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

    public async Task<SpeechSynthesisResult> SynthesizeAsync(
        string text,
        AvatarResponseSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentNullException.ThrowIfNull(settings);
        cancellationToken.ThrowIfCancellationRequested();

        var synthesizer = new SpeechSynthesizer();
        var output = new MemoryStream();
        var cues = new ConcurrentQueue<SpeechAnimationCue>();
        var textCues = new ConcurrentQueue<SpeechTextCue>();
        var completion = new TaskCompletionSource<SpeakCompletedEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler<SpeakCompletedEventArgs> completedHandler = (_, eventArgs) => completion.TrySetResult(eventArgs);
        EventHandler<PhonemeReachedEventArgs> phonemeHandler = (_, eventArgs) => cues.Enqueue(new SpeechAnimationCue(
            eventArgs.AudioPosition,
            eventArgs.Duration,
            SpeechAnimationCueType.Phoneme,
            phoneme: eventArgs.Phoneme,
            nextPhoneme: eventArgs.NextPhoneme));
        EventHandler<VisemeReachedEventArgs> visemeHandler = (_, eventArgs) => cues.Enqueue(new SpeechAnimationCue(
            eventArgs.AudioPosition,
            eventArgs.Duration,
            SpeechAnimationCueType.Viseme,
            viseme: eventArgs.Viseme,
            nextViseme: eventArgs.NextViseme));
        EventHandler<SpeakProgressEventArgs> progressHandler = (_, eventArgs) => textCues.Enqueue(new SpeechTextCue(
            eventArgs.AudioPosition,
            eventArgs.CharacterPosition,
            eventArgs.CharacterCount));
        synthesizer.SpeakCompleted += completedHandler;
        synthesizer.PhonemeReached += phonemeHandler;
        synthesizer.VisemeReached += visemeHandler;
        synthesizer.SpeakProgress += progressHandler;

        try
        {
            if (!string.IsNullOrWhiteSpace(settings.VoiceName))
            {
                synthesizer.SelectVoice(settings.VoiceName);
            }

            synthesizer.Volume = settings.Volume;
            synthesizer.Rate = settings.Rate;
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
            return new SpeechSynthesisResult(output, new SpeechAnimationTrack(cues, textCues));
        }
        catch
        {
            output.Dispose();
            throw;
        }
        finally
        {
            synthesizer.SpeakCompleted -= completedHandler;
            synthesizer.PhonemeReached -= phonemeHandler;
            synthesizer.VisemeReached -= visemeHandler;
            synthesizer.SpeakProgress -= progressHandler;
            synthesizer.Dispose();
        }
    }
}