using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChatAvatar.Component.Feature.AvatarResponse.Modell;

namespace ChatAvatar.Component.UI;

public partial class AvatarSettingsWindow : Window
{
    private const string DefaultVoiceOption = "Systemstandard";

    public AvatarSettingsWindow(
        IReadOnlyList<string> availableVoices,
        AvatarResponseSettings initialSettings)
    {
        ArgumentNullException.ThrowIfNull(availableVoices);
        ArgumentNullException.ThrowIfNull(initialSettings);

        InitializeComponent();
        VoiceInput.ItemsSource = new[] { DefaultVoiceOption }
            .Concat(availableVoices.Where(voice => !string.IsNullOrWhiteSpace(voice)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        VoiceInput.SelectedItem = initialSettings.VoiceName ?? DefaultVoiceOption;
        AvatarIdInput.Text = initialSettings.AvatarId ?? string.Empty;
        VolumeInput.Value = initialSettings.Volume;
        RateInput.Value = initialSettings.Rate;
        UpdateSliderLabels();
    }

    public AvatarResponseSettings? ResultSettings { get; private set; }

    private void OnVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdateSliderLabels();

    private void OnRateChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdateSliderLabels();

    private void OnApplyClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var voiceName = string.Equals(VoiceInput.SelectedItem as string, DefaultVoiceOption, StringComparison.Ordinal)
                ? null
                : VoiceInput.SelectedItem as string;
            ResultSettings = new AvatarResponseSettings(
                voiceName,
                AvatarIdInput.Text,
                (int)VolumeInput.Value,
                (int)RateInput.Value);
            DialogResult = true;
        }
        catch (ArgumentException exception)
        {
            ValidationLabel.Text = exception.Message;
        }
    }

    private void UpdateSliderLabels()
    {
        if (VolumeValue is not null)
        {
            VolumeValue.Text = $"{(int)VolumeInput.Value}%";
        }

        if (RateValue is not null)
        {
            RateValue.Text = ((int)RateInput.Value).ToString("+#;-#;0");
        }
    }
}