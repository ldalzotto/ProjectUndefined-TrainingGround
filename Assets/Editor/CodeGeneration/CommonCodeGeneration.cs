using System.IO;

public static class CommonCodeGeneration
{
    public static DirectoryInfo CreateModuleConfigurationFolderIfNecessary(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        var puzzleConfigurationFodler = new DirectoryInfo(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName));
        if (!puzzleConfigurationFodler.Exists)
        {
            puzzleConfigurationFodler.Create();
        }
        var puzzleConfigurationDataFodler = new DirectoryInfo(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName) + "/Data");
        if (!puzzleConfigurationDataFodler.Exists)
        {
            puzzleConfigurationDataFodler.Create();
        }
        return puzzleConfigurationFodler;
    }
}
