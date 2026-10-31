using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using ChatAvatar.Component.Featur.Avatar.Service;
using Microsoft.Web.WebView2.Core;

namespace ChatAvatar.Component.UI;

public partial class AvatarWebView : UserControl, IAvatarRenderer
{
    private const string VirtualHostName = "chatavatar.local";
    private readonly List<AvatarWebMessage> _pendingMessages = [];
    private bool _isPageReady;
    private static readonly JsonSerializerOptions MessageSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter<AvatarWebMessageType>(),
            new JsonStringEnumConverter<AvatarKind>()
        }
    };

    public AvatarWebView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public event EventHandler<AvatarWebMessageEventArgs>? MessageReceived;

    public async Task SendMessageAsync(AvatarWebMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        await Dispatcher.InvokeAsync(() =>
        {
            if (Browser.CoreWebView2 is null || !_isPageReady)
            {
                _pendingMessages.RemoveAll(pending => pending.Type == message.Type);
                _pendingMessages.Add(message);
                return;
            }

            PostMessage(message);
        });
    }

    public Task SetVisemeAsync(int visemeId)
    {
        if (visemeId is < 0 or > 21)
        {
            throw new ArgumentOutOfRangeException(nameof(visemeId), "Viseme IDs must be between 0 and 21.");
        }

        return SendMessageAsync(new AvatarWebMessage(
            AvatarWebMessageType.SetViseme,
            AvatarWebMessage.CurrentProtocolVersion,
            visemeId));
    }

    public Task ResetSpeechAnimationAsync() => SendMessageAsync(new AvatarWebMessage(
        AvatarWebMessageType.ResetSpeechAnimation,
        AvatarWebMessage.CurrentProtocolVersion));

    public Task SetAvatarTypeAsync(AvatarKind avatarType)
    {
        if (!Enum.IsDefined(avatarType))
        {
            throw new ArgumentOutOfRangeException(nameof(avatarType));
        }

        return SendMessageAsync(new AvatarWebMessage(
            AvatarWebMessageType.SetAvatarType,
            AvatarWebMessage.CurrentProtocolVersion,
            AvatarType: avatarType));
    }

    public Task SetAudioLevelAsync(float audioLevel)
    {
        if (!float.IsFinite(audioLevel) || audioLevel is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(audioLevel), "Audio level must be between 0 and 1.");
        }

        return SendMessageAsync(new AvatarWebMessage(
            AvatarWebMessageType.SetAudioLevel,
            AvatarWebMessage.CurrentProtocolVersion,
            AudioLevel: audioLevel));
    }

    public Task SetAvatarAppearanceAsync(AvatarDefinition avatar)
    {
        return SendMessageAsync(AvatarWebMessage.ForAppearance(avatar));
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        try
        {
            var pagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "WebView", "index.html");
            if (!File.Exists(pagePath))
            {
                throw new FileNotFoundException("The local avatar page was not found.", pagePath);
            }

            var runtimeDirectory = WebView2RuntimeLocator.FindBundledRuntime(AppContext.BaseDirectory);
            var userDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ChatAvatar",
                "WebView2");
            Directory.CreateDirectory(userDataDirectory);
            var environment = await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: runtimeDirectory,
                userDataFolder: userDataDirectory);
            await Browser.EnsureCoreWebView2Async(environment);
            Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(
                VirtualHostName,
                AppContext.BaseDirectory,
                CoreWebView2HostResourceAccessKind.DenyCors);
            Browser.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            Browser.Source = new Uri($"https://{VirtualHostName}/Assets/WebView/index.html");
        }
        catch (Exception exception)
        {
            ShowError($"The avatar view could not be started. Verify that the WebView2 Runtime is installed. {exception.Message}");
        }
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps
            || !string.Equals(uri.Host, VirtualHostName, StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            ShowError("The avatar view blocked a navigation outside its local files.");
        }
    }

    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (!e.IsSuccess)
        {
            ShowError($"The local avatar page could not be loaded: {e.WebErrorStatus}.");
            return;
        }

        ErrorPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowError(string message)
    {
        ErrorMessage.Text = message;
        ErrorPanel.Visibility = Visibility.Visible;
    }

    private async void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            var message = JsonSerializer.Deserialize<AvatarWebMessage>(e.WebMessageAsJson, MessageSerializerOptions)
                ?? throw new JsonException("The web view sent an empty message.");
            if (message.ProtocolVersion != AvatarWebMessage.CurrentProtocolVersion)
            {
                throw new JsonException($"Unsupported WebView message protocol: {message.ProtocolVersion}.");
            }

            if (message.Type == AvatarWebMessageType.Ready)
            {
                _isPageReady = true;
                await SendMessageAsync(new AvatarWebMessage(
                    AvatarWebMessageType.Initialize,
                    AvatarWebMessage.CurrentProtocolVersion));
                foreach (var pendingMessage in _pendingMessages)
                {
                    PostMessage(pendingMessage);
                }

                _pendingMessages.Clear();
            }

            MessageReceived?.Invoke(this, new AvatarWebMessageEventArgs(message));
        }
        catch (JsonException exception)
        {
            ShowError($"The local avatar page sent an invalid message. {exception.Message}");
        }
    }

    private void PostMessage(AvatarWebMessage message) =>
        Browser.CoreWebView2!.PostWebMessageAsJson(JsonSerializer.Serialize(message, MessageSerializerOptions));
}