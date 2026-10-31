namespace ChatAvatar.Component.Feature.Avatar.Modell;

public sealed record AvatarFileDocument
{
    public const int CurrentSchemaVersion = 1;

    public AvatarFileDocument(int schemaVersion, AvatarDefinition avatar)
    {
        if (schemaVersion != CurrentSchemaVersion)
        {
            throw new ArgumentOutOfRangeException(
                nameof(schemaVersion),
                schemaVersion,
                $"Only avatar schema version {CurrentSchemaVersion} is supported.");
        }

        ArgumentNullException.ThrowIfNull(avatar);

        SchemaVersion = schemaVersion;
        Avatar = avatar;
    }

    public int SchemaVersion { get; }

    public AvatarDefinition Avatar { get; }
}