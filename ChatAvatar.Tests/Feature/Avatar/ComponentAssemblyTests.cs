using System.Reflection;

namespace ChatAvatar.Tests.Feature.Avatar;

[TestClass]
public sealed class ComponentAssemblyTests
{
    [TestMethod]
    public void ComponentAssemblyCanBeLoadedThroughProjectReference()
    {
        var assembly = Assembly.Load("ChatAvatar.Component");

        Assert.AreEqual("ChatAvatar.Component", assembly.GetName().Name);
    }
}