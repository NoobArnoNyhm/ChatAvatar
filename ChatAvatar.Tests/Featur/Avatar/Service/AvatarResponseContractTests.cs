using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class AvatarResponseContractTests
{
    [TestMethod]
    public void RequestPreservesTextAndHostSettings()
    {
        var settings = new AvatarResponseSettings("Local Voice", "avatar-1");
        var request = new AvatarResponseRequest("Hello", settings);

        Assert.AreEqual("Hello", request.Text);
        Assert.AreSame(settings, request.Settings);
    }

    [TestMethod]
    public void RequestRejectsEmptyText()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AvatarResponseRequest(string.Empty));
    }

    [TestMethod]
    public void RequestRejectsWhitespaceText()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AvatarResponseRequest(" "));
    }

    [TestMethod]
    public async Task SessionCancellationCompletesWithItsResponseId()
    {
        using var session = new AvatarResponseSession();

        Assert.IsTrue(session.Cancel());

        var result = await session.Completion;

        Assert.AreEqual(session.ResponseId, result.ResponseId);
        Assert.AreEqual(AvatarResponseStatus.Cancelled, result.Status);
        Assert.IsTrue(session.CancellationToken.IsCancellationRequested);
        Assert.IsFalse(session.Cancel());
    }

    [TestMethod]
    public async Task SessionCanCompleteOnlyOnce()
    {
        using var session = new AvatarResponseSession();

        Assert.IsTrue(session.Complete());
        Assert.IsFalse(session.Fail("late failure"));

        var result = await session.Completion;

        Assert.AreEqual(session.ResponseId, result.ResponseId);
        Assert.AreEqual(AvatarResponseStatus.Completed, result.Status);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task SessionReportsFailure()
    {
        using var session = new AvatarResponseSession();

        Assert.IsTrue(session.Fail("Synthesis failed."));

        var result = await session.Completion;

        Assert.AreEqual(AvatarResponseStatus.Failed, result.Status);
        Assert.AreEqual("Synthesis failed.", result.ErrorMessage);
    }
}