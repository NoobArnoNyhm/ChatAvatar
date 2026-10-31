namespace ChatAvatar.Component.Feature.Avatar.Service;

public interface IAudioLevelAnalyzer
{
    float GetRmsLevel(TimeSpan audioPosition, TimeSpan? window = null);
}