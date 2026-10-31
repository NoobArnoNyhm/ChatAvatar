namespace ChatAvatar.Component.Feature.AudioPlayback.Service;

public interface IAudioLevelAnalyzer
{
    float GetRmsLevel(TimeSpan audioPosition, TimeSpan? window = null);
}