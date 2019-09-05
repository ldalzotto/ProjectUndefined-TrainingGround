using System.IO;

public static class CommonCodeGeneration
{
    public static DirectoryInfo CreatePuzzleSubConfigurationFolderIfNecessary(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        var puzzleConfigurationFodler = new DirectoryInfo(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName));
        if (!puzzleConfigurationFodler.Exists)
        {
            puzzleConfigurationFodler.Create();
        }
        return puzzleConfigurationFodler;
    }
}
