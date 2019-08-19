using UnityEngine;
using System.Collections;
using System.IO;

public static class CommonCodeGeneration
{
    public static DirectoryInfo CreatePuzzleSubConfigurationFolderIfNecessary(string baseName)
    {
        var puzzleConfigurationFodler = new DirectoryInfo(PathConstants.PuzzleSubConfigurationFolderPath + "/" + baseName + "Configuration");
        if (!puzzleConfigurationFodler.Exists)
        {
            puzzleConfigurationFodler.Create();
        }
        return puzzleConfigurationFodler;
    }
}
