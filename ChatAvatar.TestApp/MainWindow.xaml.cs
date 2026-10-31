using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using ChatAvatar.Component.Featur.Avatar.Service;
using ChatAvatar.Component.UI;

namespace ChatAvatar.TestApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string _avatarDirectory;
    private readonly AvatarDefinitionFileStore _avatarFileStore = new();
    private readonly AvatarCatalog _avatarCatalog;
    private readonly AvatarRuntimeService _avatarRuntime;
    private readonly SystemSpeechSynthesisEngine _speechEngine = new();
    private readonly SpeechSynthesisService _speechSynthesisService;
    private readonly SpeechPlaybackCoordinator _playbackCoordinator = new();
    private readonly TestHostSettingsStore _settingsStore;
    private readonly ObservableCollection<AvatarCatalogEntry> _avatars = [];
    private AvatarResponseSettings _settings = new();
    private AvatarResponseSession? _activeResponse;
    private AudioPlaybackSession? _activePlayback;
    private CancellationTokenSource? _activeCancellation;
    private bool _initializing;
    private readonly bool _renderSmokeTest;
    private readonly string? _renderSmokeReportPath;

    public MainWindow()
    {
        InitializeComponent();
        _renderSmokeTest = Environment.GetCommandLineArgs().Contains("--render-smoke-test", StringComparer.Ordinal);
        _renderSmokeReportPath = Environment.GetEnvironmentVariable("CHATAVATAR_RENDER_SMOKE_REPORT");

        var hostDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ChatAvatar");
        _avatarDirectory = Path.Combine(hostDirectory, "Avatars");
        _settingsStore = new TestHostSettingsStore(Path.Combine(hostDirectory, "settings.json"));
        _avatarCatalog = new AvatarCatalog(_avatarFileStore);
        _avatarRuntime = new AvatarRuntimeService(_avatarCatalog, AvatarView);
        _speechSynthesisService = new SpeechSynthesisService(_speechEngine);
        _playbackCoordinator.FrameChanged += OnPlaybackFrameChanged;
        AvatarView.MessageReceived += OnAvatarViewMessageReceived;
        AvatarPicker.ItemsSource = _avatars;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _initializing = true;
        try
        {
            Directory.CreateDirectory(_avatarDirectory);
            CopySampleAvatars();
            _settings = await _settingsStore.LoadAsync();
            var result = await _avatarCatalog.ListAsync(_avatarDirectory);
            _avatars.Clear();
            foreach (var entry in result.Entries)
            {
                _avatars.Add(entry);
            }

            var savedAvatar = _avatars.FirstOrDefault(entry =>
                string.Equals(entry.Avatar.Id.ToString(), _settings.AvatarId, StringComparison.OrdinalIgnoreCase));
            AvatarPicker.SelectedItem = savedAvatar ?? _avatars.FirstOrDefault();
            if (AvatarPicker.SelectedItem is AvatarCatalogEntry selected)
            {
                await ActivateAvatarAsync(selected, persistSettings: false);
            }

            StatusText.Text = result.Issues.Count == 0
                ? $"Bereit. {_avatars.Count} Beispielavatar(s) geladen."
                : $"Bereit; {result.Issues.Count} Avatar-Datei(en) konnten nicht geladen werden.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Testhost konnte nicht initialisiert werden: {exception.Message}";
            if (_renderSmokeTest)
            {
                WriteRenderSmokeReport(false, exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
        finally
        {
            _initializing = false;
        }
    }

    private void CopySampleAvatars()
    {
        var sampleDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Avatars");
        if (!Directory.Exists(sampleDirectory))
        {
            throw new DirectoryNotFoundException($"The local sample avatar directory was not found: {sampleDirectory}");
        }

        foreach (var samplePath in Directory.EnumerateFiles(sampleDirectory, $"*{AvatarDefinitionFileStore.FileExtension}"))
        {
            var targetPath = Path.Combine(_avatarDirectory, Path.GetFileName(samplePath));
            if (!File.Exists(targetPath))
            {
                File.Copy(samplePath, targetPath);
            }
        }
    }

    private async void OnAvatarSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_initializing || AvatarPicker.SelectedItem is not AvatarCatalogEntry entry)
        {
            return;
        }

        await ActivateAvatarAsync(entry, persistSettings: true);
    }

    private async Task ActivateAvatarAsync(AvatarCatalogEntry entry, bool persistSettings)
    {
        try
        {
            var avatar = await _avatarRuntime.SelectAsync(_avatarDirectory, entry.Avatar.Id);
            SelectedAvatarId.Text = $"ID: {avatar.Id}";
            _settings = new AvatarResponseSettings(
                _settings.VoiceName,
                avatar.Id.ToString(),
                _settings.Volume,
                _settings.Rate);
            if (persistSettings)
            {
                await _settingsStore.SaveAsync(_settings);
            }

            StatusText.Text = $"{avatar.DisplayName} aktiv.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Avatar konnte nicht aktiviert werden: {exception.Message}";
        }
    }

    private async void OnSendClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputText.Text))
        {
            StatusText.Text = "Bitte zuerst einen Text eingeben.";
            return;
        }

        if (AvatarPicker.SelectedItem is not AvatarCatalogEntry selectedAvatar)
        {
            StatusText.Text = "Es ist kein Avatar ausgewählt.";
            return;
        }

        var transcript = InputText.Text;
        var response = new AvatarResponseSession();
        using var cancellation = new CancellationTokenSource();
        _activeResponse = response;
        _activeCancellation = cancellation;
        SendButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
        StatusText.Text = "Sprache wird erzeugt...";

        SpeechSynthesisResult? synthesis = null;
        try
        {
            _settings = new AvatarResponseSettings(
                _settings.VoiceName,
                selectedAvatar.Avatar.Id.ToString(),
                _settings.Volume,
                _settings.Rate);
            await _settingsStore.SaveAsync(_settings, cancellation.Token);
            var request = new AvatarResponseRequest(transcript, _settings);
            synthesis = await _speechSynthesisService.SynthesizeAsync(request, cancellation.Token);
            var avatar = await _avatarRuntime.SelectAsync(_avatarDirectory, selectedAvatar.Avatar.Id);
            SelectedAvatarId.Text = $"ID: {avatar.Id}";
            using var playback = new AudioPlaybackSession(new SystemSoundPlayerEngine());
            _activePlayback = playback;
            StatusText.Text = "Wiedergabe läuft...";
            var result = await _playbackCoordinator.PlayAsync(transcript, synthesis, playback, cancellation.Token);

            if (result.Status == AudioPlaybackStatus.Completed)
            {
                response.Complete();
                StatusText.Text = "Antwort abgeschlossen.";
            }
            else if (result.Status == AudioPlaybackStatus.Cancelled)
            {
                response.Cancel();
                StatusText.Text = "Antwort abgebrochen.";
            }
            else
            {
                response.Fail(result.ErrorMessage ?? "Audio playback failed.");
                StatusText.Text = $"Wiedergabe fehlgeschlagen: {result.ErrorMessage}";
            }
        }
        catch (OperationCanceledException)
        {
            response.Cancel();
            StatusText.Text = "Antwort abgebrochen.";
        }
        catch (Exception exception)
        {
            response.Fail(exception.Message);
            StatusText.Text = $"Antwort fehlgeschlagen: {exception.Message}";
        }
        finally
        {
            if (synthesis is not null)
            {
                await synthesis.AudioStream.DisposeAsync();
            }

            response.Dispose();
            _activeResponse = null;
            _activePlayback = null;
            _activeCancellation = null;
            SendButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
        }
    }

    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        _activeResponse?.Cancel();
        _activePlayback?.Cancel();
        _activeCancellation?.Cancel();
    }

    private async void OnSettingsClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var window = new AvatarSettingsWindow(_speechEngine.GetInstalledVoiceNames(), _settings)
            {
                Owner = this
            };
            if (window.ShowDialog() == true && window.ResultSettings is { } settings)
            {
                _settings = new AvatarResponseSettings(
                    settings.VoiceName,
                    AvatarPicker.SelectedItem is AvatarCatalogEntry entry ? entry.Avatar.Id.ToString() : settings.AvatarId,
                    settings.Volume,
                    settings.Rate);
                await _settingsStore.SaveAsync(_settings);
                StatusText.Text = "Einstellungen vom Testhost gespeichert.";
            }
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Einstellungen konnten nicht geladen werden: {exception.Message}";
        }
    }

    private void OnEditorClicked(object sender, RoutedEventArgs e)
    {
        var window = new AvatarEditorWindow(
            new AvatarEditorService(_avatarCatalog, _avatarFileStore),
            _avatarDirectory)
        {
            Owner = this
        };
        window.ShowDialog();
        _ = RefreshAvatarsAsync();
    }

    private async Task RefreshAvatarsAsync()
    {
        var result = await _avatarCatalog.ListAsync(_avatarDirectory);
        var selectedId = (AvatarPicker.SelectedItem as AvatarCatalogEntry)?.Avatar.Id;
        _initializing = true;
        try
        {
            _avatars.Clear();
            foreach (var entry in result.Entries)
            {
                _avatars.Add(entry);
            }

            AvatarPicker.SelectedItem = _avatars.FirstOrDefault(entry => entry.Avatar.Id == selectedId)
                ?? _avatars.FirstOrDefault();
        }
        finally
        {
            _initializing = false;
        }

        if (AvatarPicker.SelectedItem is AvatarCatalogEntry selected)
        {
            await ActivateAvatarAsync(selected, persistSettings: false);
        }
    }

    private async void OnPlaybackFrameChanged(object? sender, SpeechPlaybackFrameEventArgs e)
    {
        var frame = e.Frame;
        if (ReferenceEquals(frame, SpeechPlaybackFrame.Empty))
        {
            ActiveSpeechText.Text = string.Empty;
            TranscriptText.Text = string.Empty;
            await AvatarView.ResetSpeechAnimationAsync();
            await AvatarView.SetAudioLevelAsync(0);
            return;
        }

        TranscriptText.Text = frame.Transcript;
        ActiveSpeechText.Text = frame.ActiveText;
        foreach (var cue in frame.DueAnimationCues)
        {
            if (cue.Type == SpeechAnimationCueType.Viseme && cue.Viseme is int viseme)
            {
                await AvatarView.SetVisemeAsync(viseme);
            }
        }

        await AvatarView.SetAudioLevelAsync(frame.AudioLevel);
    }

    private void OnAvatarViewMessageReceived(object? sender, AvatarWebMessageEventArgs e)
    {
        if (e.Message.Type == AvatarWebMessageType.SceneReady)
        {
            StatusText.Text = $"3D-Szene gerendert ({e.Message.CanvasWidth}x{e.Message.CanvasHeight}).";
            if (_renderSmokeTest)
            {
                WriteRenderSmokeReport(true, StatusText.Text);
                Application.Current.Shutdown(0);
            }
        }
        else if (e.Message.Type == AvatarWebMessageType.RendererError)
        {
            StatusText.Text = $"Avatar-Rendererfehler: {e.Message.ErrorMessage}";
            if (_renderSmokeTest)
            {
                WriteRenderSmokeReport(false, e.Message.ErrorMessage ?? StatusText.Text);
                Application.Current.Shutdown(1);
            }
        }
    }

    private void WriteRenderSmokeReport(bool sceneReady, string details)
    {
        if (string.IsNullOrWhiteSpace(_renderSmokeReportPath))
        {
            return;
        }

        File.WriteAllText(_renderSmokeReportPath, System.Text.Json.JsonSerializer.Serialize(new
        {
            sceneReady,
            details
        }));
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        _activeResponse?.Cancel();
        _activePlayback?.Cancel();
        _activeCancellation?.Cancel();
    }
}