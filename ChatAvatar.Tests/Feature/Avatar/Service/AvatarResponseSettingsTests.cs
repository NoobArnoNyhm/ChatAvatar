using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.Service;

[TestClass]
public sealed class AvatarResponseSettingsTests
{
    [TestMethod]
    public void DefaultsUseSystemVoiceMaximumVolumeAndNormalRate()
    {
        var settings = new AvatarResponseSettings();

        Assert.IsNull(settings.VoiceName);
        Assert.IsNull(settings.AvatarId);
        Assert.AreEqual(100, settings.Volume);
        Assert.AreEqual(0, settings.Rate);
    }

    [TestMethod]
    public void SettingsCanBeReturnedThroughTheResponseRequest()
    {
        var settings = new AvatarResponseSettings("Local Voice", "avatar-id", 70, -2);
        var request = new AvatarResponseRequest("Hello", settings);

        Assert.AreSame(settings, request.Settings);
        Assert.AreEqual("Local Voice", request.Settings.VoiceName);
        Assert.AreEqual("avatar-id", request.Settings.AvatarId);
        Assert.AreEqual(70, request.Settings.Volume);
        Assert.AreEqual(-2, request.Settings.Rate);
    }

    [TestMethod]
    public void SettingsRejectValuesOutsideSystemSpeechRanges()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new AvatarResponseSettings(volume: -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new AvatarResponseSettings(volume: 101));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new AvatarResponseSettings(rate: -11));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new AvatarResponseSettings(rate: 11));
    }
}