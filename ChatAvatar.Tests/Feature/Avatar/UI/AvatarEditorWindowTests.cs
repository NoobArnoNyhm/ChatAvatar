using System.IO;
using ChatAvatar.Component.UI;
using ChatAvatar.Component.Feature.Avatar.Service;

namespace ChatAvatar.Tests.Feature.Avatar.UI;

[TestClass]
public sealed class AvatarEditorWindowTests
{
    [STATestMethod]
    public void EditorWindowInitializesWithoutEarlyAppearanceChangeEventsFailing()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"ChatAvatarEditorWindowTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            var fileStore = new AvatarDefinitionFileStore();
            var service = new AvatarEditorService(new AvatarCatalog(fileStore), fileStore);
            var window = new AvatarEditorWindow(service, directory);

            window.Close();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}