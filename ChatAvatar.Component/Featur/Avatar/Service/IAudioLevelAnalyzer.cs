namespace ChatAvatar.Component.Featur.Avatar.Service;

public interface IAudioLevelAnalyzer
{
    float GetRmsLevel(TimeSpan audioPosition, TimeSpan? window = null);
}