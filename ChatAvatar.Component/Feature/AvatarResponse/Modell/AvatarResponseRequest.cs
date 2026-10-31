namespace ChatAvatar.Component.Feature.AvatarResponse.Modell;

public sealed class AvatarResponseRequest
{
    public AvatarResponseRequest(string text, AvatarResponseSettings? settings = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Text = text;
        Settings = settings ?? new AvatarResponseSettings();
    }

    public string Text { get; }

    public AvatarResponseSettings Settings { get; }
}