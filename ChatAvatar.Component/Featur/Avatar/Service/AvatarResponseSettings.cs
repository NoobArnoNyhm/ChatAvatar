namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed record AvatarResponseSettings
{
	public AvatarResponseSettings(
		string? voiceName = null,
		string? avatarId = null,
		int volume = 100,
		int rate = 0)
	{
		if (volume is < 0 or > 100)
		{
			throw new ArgumentOutOfRangeException(nameof(volume), "Voice volume must be between 0 and 100.");
		}

		if (rate is < -10 or > 10)
		{
			throw new ArgumentOutOfRangeException(nameof(rate), "Speech rate must be between -10 and 10.");
		}

		VoiceName = string.IsNullOrWhiteSpace(voiceName) ? null : voiceName;
		AvatarId = string.IsNullOrWhiteSpace(avatarId) ? null : avatarId;
		Volume = volume;
		Rate = rate;
	}

	public string? VoiceName { get; }

	public string? AvatarId { get; }

	public int Volume { get; }

	public int Rate { get; }
}