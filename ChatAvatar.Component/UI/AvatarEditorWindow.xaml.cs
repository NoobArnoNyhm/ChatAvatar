using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Component.UI;

public partial class AvatarEditorWindow : Window
{
    private readonly AvatarEditorService _editorService;
    private readonly string _avatarDirectory;
    private AvatarDefinition? _currentAvatar;

    public AvatarEditorWindow(AvatarEditorService editorService, string avatarDirectory)
    {
        ArgumentNullException.ThrowIfNull(editorService);
        ArgumentException.ThrowIfNullOrWhiteSpace(avatarDirectory);

        InitializeComponent();
        _editorService = editorService;
        _avatarDirectory = avatarDirectory;
        AvatarTypeInput.ItemsSource = Enum.GetValues<AvatarKind>();
        AvatarTypeInput.SelectedItem = AvatarKind.Face;
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
        NameInput.Text = _currentAvatar.DisplayName;
        AvatarTypeInput.SelectedItem = _currentAvatar.Kind;
        AvatarIdLabel.Text = _currentAvatar.Id.ToString();
        StatusLabel.Text = "Neuer Avatar. Nach dem Speichern erscheint er links in der Liste.";
        AvatarList.SelectedItem = null;
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
            NameInput.Text = _currentAvatar.DisplayName;
            AvatarTypeInput.SelectedItem = _currentAvatar.Kind;
            AvatarIdLabel.Text = _currentAvatar.Id.ToString();
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

        try
        {
            var appearance = _currentAvatar.Kind == kind
                ? _currentAvatar.Appearance
                : AvatarEditorService.CreateDefaultAppearance(kind);
            _currentAvatar = new AvatarDefinition(_currentAvatar.Id, NameInput.Text, appearance);
            await _editorService.SaveAsync(_avatarDirectory, _currentAvatar);
            await RefreshAsync();
            AvatarList.SelectedItem = Avatars.FirstOrDefault(entry => entry.Avatar.Id == _currentAvatar.Id);
            StatusLabel.Text = $"{_currentAvatar.DisplayName} gespeichert.";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Avatar konnte nicht gespeichert werden: {exception.Message}";
        }
    }
}