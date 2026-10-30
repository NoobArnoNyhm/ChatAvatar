using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ChatAvatar.Component.UI;

public partial class AvatarWebView : UserControl
{
    public AvatarWebView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
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

            await Browser.EnsureCoreWebView2Async();
            Browser.Source = new Uri(pagePath);
        }
        catch (Exception exception)
        {
            ShowError($"The avatar view could not be started. Verify that the WebView2 Runtime is installed. {exception.Message}");
        }
    }

    private void OnNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeFile)
        {
            e.Cancel = true;
            ShowError("The avatar view blocked a navigation outside its local files.");
        }
    }

    private void OnNavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
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
}