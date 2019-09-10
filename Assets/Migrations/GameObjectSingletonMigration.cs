#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class GameObjectSingletonMigration : EditorWindow
{
    [MenuItem("Migration/GameObjectSingletonMigration")]
    public static void ShowWindow()
    {
        var GameObjectSingletonMigration = EditorWindow.GetWindow(typeof(GameObjectSingletonMigration));
        GameObjectSingletonMigration.Show();
    }

    private const string Path = "C:\\Users\\Loic\\Documents\\projects\\SoulsLike\\No Entry_0.0.71_Reset\\Assets\\~RTPuzzleGame";


    private void OnGUI()
    {
        if (GUILayout.Button("MIGRATE"))
        {
            var gameObjectRegex = new Regex("(GameObject\\.FindObjectOfType<)(.+(?=>))(.*?[)])");
            FileInfo[] files = new DirectoryInfo(Path).GetFiles("*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file.FullName);
                var fileMatche = gameObjectRegex.Match(fileContent);

                if (!fileMatche.Success)
                {
                    continue;
                }

                while (fileMatche.Success)
                {
                    fileContent = fileContent.Replace(fileMatche.Groups[0].Value, "PuzzleGameSingletonInstances." + fileMatche.Groups[2].Value);

                    fileMatche = gameObjectRegex.Match(fileContent);
                }

                File.WriteAllText(file.FullName, fileContent);
            }
        }
    }
}
#endif