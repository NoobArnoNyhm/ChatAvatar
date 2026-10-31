using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;
using ChatAvatar.Component.UI;

namespace ChatAvatar.Tests.Featur.Avatar.UI;

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