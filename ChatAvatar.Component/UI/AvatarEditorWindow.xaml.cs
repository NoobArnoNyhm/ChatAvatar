using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChatAvatar.Component.Feature.Avatar.Modell;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Component.UI;

public partial class AvatarEditorWindow : Window
{
    private readonly AvatarEditorService _editorService;
    private readonly string _avatarDirectory;
    private AvatarDefinition? _currentAvatar;
    private bool _isLoadingAppearance;
    private bool _isInitialized;

    public AvatarEditorWindow(AvatarEditorService editorService, string avatarDirectory)
    {
        ArgumentNullException.ThrowIfNull(editorService);
        ArgumentException.ThrowIfNullOrWhiteSpace(avatarDirectory);

        InitializeComponent();
        _isInitialized = true;
        _editorService = editorService;
        _avatarDirectory = avatarDirectory;
        AvatarTypeInput.ItemsSource = Enum.GetValues<AvatarKind>();
        AvatarTypeInput.SelectedItem = AvatarKind.Face;
        TexturePathInput.Text = "builtin:image-style-lens";
        TintColorInput.Text = "#E2A56F";
        LightColorInput.Text = "#FFFFFF";
        MouthScaleValue.Text = "1.00";
        PulseIntensityValue.Text = "0.65";
        DataContext = this;
        Loaded += async (_, _) => await RefreshAsync();
    }

    public ObservableCollection<AvatarCatalogEntry> Avatars { get; } = [];

    public AvatarCatalogEntry? SelectedAvatar { get; set; }

    private async Task RefreshAsync()
    {
        try
        {
            var result = await _editorService.ListAsync(_avatarDirectory);
            Avatars.Clear();
            foreach (var entry in result.Entries)
            {
                Avatars.Add(entry);
            }

            StatusLabel.Text = result.Issues.Count == 0
                ? $"{result.Entries.Count} Avatar(s) geladen."
                : $"{result.Entries.Count} Avatar(s) geladen; {result.Issues.Count} Datei(en) konnten nicht gelesen werden.";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Avatare konnten nicht geladen werden: {exception.Message}";
        }
    }

    private async void OnRefreshClicked(object sender, RoutedEventArgs e) => await RefreshAsync();

    private void OnNewClicked(object sender, RoutedEventArgs e)
    {
        var kind = AvatarTypeInput.SelectedItem is AvatarKind selectedKind ? selectedKind : AvatarKind.Face;
        _currentAvatar = _editorService.CreateNew("Neuer Avatar", kind);
        _isLoadingAppearance = true;
        try
        {
            NameInput.Text = _currentAvatar.DisplayName;
            LoadAppearanceIntoEditor(_currentAvatar);
            AvatarIdLabel.Text = _currentAvatar.Id.ToString();
        }
        finally
        {
            _isLoadingAppearance = false;
        }
        StatusLabel.Text = "Neuer Avatar. Nach dem Speichern erscheint er links in der Liste.";
        AvatarList.SelectedItem = null;
        _ = OnAppearanceChangedAsync();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AvatarList.SelectedItem is not AvatarCatalogEntry entry)
        {
            return;
        }

        try
        {
            _currentAvatar = await _editorService.OpenAsync(_avatarDirectory, entry.Avatar.Id);
            _isLoadingAppearance = true;
            try
            {
                NameInput.Text = _currentAvatar.DisplayName;
                LoadAppearanceIntoEditor(_currentAvatar);
                AvatarIdLabel.Text = _currentAvatar.Id.ToString();
            }
            finally
            {
                _isLoadingAppearance = false;
            }

            await Preview.SetAvatarAppearanceAsync(_currentAvatar);
            StatusLabel.Text = $"{_currentAvatar.DisplayName} geöffnet.";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Avatar konnte nicht geöffnet werden: {exception.Message}";
        }
    }

    private async void OnSaveClicked(object sender, RoutedEventArgs e)
    {
        if (_currentAvatar is null || AvatarTypeInput.SelectedItem is not AvatarKind kind)
        {
            StatusLabel.Text = "Zuerst einen Avatar anlegen oder öffnen.";
            return;
        }

        if (!TryCreateDraft(out var draft))
        {
            return;
        }

        try
        {
            _currentAvatar = draft;
            await _editorService.SaveAsync(_avatarDirectory, draft);
            await RefreshAsync();
            AvatarList.SelectedItem = Avatars.FirstOrDefault(entry => entry.Avatar.Id == draft.Id);
            StatusLabel.Text = $"{draft.DisplayName} gespeichert.";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Avatar konnte nicht gespeichert werden: {exception.Message}";
        }
    }

    private void OnAvatarTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoadingAppearance)
        {
            return;
        }

        var kind = AvatarTypeInput.SelectedItem is AvatarKind selectedKind ? selectedKind : AvatarKind.Face;
        FaceSettingsPanel.Visibility = kind == AvatarKind.Face ? Visibility.Visible : Visibility.Collapsed;
        ImageSettingsPanel.Visibility = kind == AvatarKind.ImageBased ? Visibility.Visible : Visibility.Collapsed;
        AbstractSettingsPanel.Visibility = kind == AvatarKind.Abstract ? Visibility.Visible : Visibility.Collapsed;
        OnAppearanceChanged(sender, e);
    }

    private async void OnAppearanceChanged(object sender, RoutedEventArgs e) => await OnAppearanceChangedAsync();

    private async Task OnAppearanceChangedAsync()
    {
        if (!_isInitialized)
        {
            return;
        }

        MouthScaleValue.Text = MouthScaleInput.Value.ToString("0.00", CultureInfo.InvariantCulture);
        PulseIntensityValue.Text = PulseIntensityInput.Value.ToString("0.00", CultureInfo.InvariantCulture);
        if (!IsLoaded || _isLoadingAppearance || _currentAvatar is null || !TryCreateDraft(out var draft))
        {
            return;
        }

        _currentAvatar = draft;
        try
        {
            await Preview.SetAvatarAppearanceAsync(draft);
        }
        catch (InvalidOperationException exception)
        {
            StatusLabel.Text = $"Vorschau konnte nicht aktualisiert werden: {exception.Message}";
        }
    }

    private bool TryCreateDraft(out AvatarDefinition draft)
    {
        draft = null!;
        if (_currentAvatar is null || AvatarTypeInput.SelectedItem is not AvatarKind kind)
        {
            return false;
        }

        try
        {
            AvatarAppearance appearance = kind switch
            {
                AvatarKind.Face => new FaceAvatarAppearance(
                    string.IsNullOrWhiteSpace(FaceModelPathInput.Text) ? null : FaceModelPathInput.Text,
                    (float)MouthScaleInput.Value),
                AvatarKind.ImageBased => new ImageBasedAvatarAppearance(TexturePathInput.Text, TintColorInput.Text),
                AvatarKind.Abstract => new AbstractAvatarAppearance(
                    LightColorInput.Text,
                    (float)PulseIntensityInput.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            draft = new AvatarDefinition(_currentAvatar.Id, NameInput.Text, appearance);
            ValidationLabel.Text = string.Empty;
            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            ValidationLabel.Text = exception.Message;
            return false;
        }
    }

    private void LoadAppearanceIntoEditor(AvatarDefinition avatar)
    {
        AvatarTypeInput.SelectedItem = avatar.Kind;
        switch (avatar.Appearance)
        {
            case FaceAvatarAppearance face:
                FaceModelPathInput.Text = face.ModelAssetPath ?? string.Empty;
                MouthScaleInput.Value = face.MouthScale;
                break;
            case ImageBasedAvatarAppearance image:
                TexturePathInput.Text = image.TextureAssetPath;
                TintColorInput.Text = image.TintColor;
                break;
            case AbstractAvatarAppearance abstractAvatar:
                LightColorInput.Text = abstractAvatar.LightColor;
                PulseIntensityInput.Value = abstractAvatar.PulseIntensity;
                break;
        }

        UpdateAppearancePanels(avatar.Kind);
        ValidationLabel.Text = string.Empty;
    }

    private void UpdateAppearancePanels(AvatarKind kind)
    {
        FaceSettingsPanel.Visibility = kind == AvatarKind.Face ? Visibility.Visible : Visibility.Collapsed;
        ImageSettingsPanel.Visibility = kind == AvatarKind.ImageBased ? Visibility.Visible : Visibility.Collapsed;
        AbstractSettingsPanel.Visibility = kind == AvatarKind.Abstract ? Visibility.Visible : Visibility.Collapsed;
    }
}