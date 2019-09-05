using AdventureGame;
using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using System.CodeDom;

public static class GameTypeCodeGenerationConfiguration
{
    public static IGameTypeCodeGenerationConfiguration Get(GameTypeGeneration GameTypeGeneration)
    {
        if (GameTypeGeneration == GameTypeGeneration.ADVENTURE)
        {
            return new AdventureGameTypeCondeGenerationConfiguration();
        }
        else if (GameTypeGeneration == GameTypeGeneration.PUZZLE)
        {
            return new PuzzleGameTypeCondeGenerationConfiguration();
        }

        return null;
    }
}

public interface IGameTypeCodeGenerationConfiguration
{
    string GetSubConfigurationFolderPath(string baseName);
    string GetConfigurationFolderPath();
    string GetGameConfigurationManagerMethodTemplatePath();
    string GetGameConfigurationManagerPath();
    CodeNamespace GetNamespace();
    string GetConfigurationAssetMenuAttributeName(string baseName, string className);
    Type GetConfigurationType();
    Type GetEditorConfigurationsType();
}

public class PuzzleGameTypeCondeGenerationConfiguration : IGameTypeCodeGenerationConfiguration
{
    public string GetSubConfigurationFolderPath(string baseName)
    {
        return PathConstants.PuzzleSubConfigurationFolderPath + "/" + baseName + "Configuration";
    }

    public string GetConfigurationFolderPath()
    {
        return PathConstants.PuzzleConfigurationFolderPath;
    }

    public string GetGameConfigurationManagerMethodTemplatePath()
    {
        return PathConstants.PuzzleGameConfigurationManagerMethodTemplatePath;
    }

    public string GetGameConfigurationManagerPath()
    {
        return PathConstants.PuzzleGameConfigurationManagerPath;
    }

    public CodeNamespace GetNamespace()
    {
        return new CodeNamespace(typeof(PuzzleEventsManager).Namespace);
    }

    public string GetConfigurationAssetMenuAttributeName(string baseName, string className)
    {
        return "Configuration/PuzzleGame/" + baseName + "Configuration/" + className;
    }

    public Type GetConfigurationType()
    {
        return typeof(PuzzleGameConfiguration);
    }

    public Type GetEditorConfigurationsType()
    {
        return typeof(PuzzleGameConfigurations);
    }
}

public class AdventureGameTypeCondeGenerationConfiguration : IGameTypeCodeGenerationConfiguration
{
    public string GetSubConfigurationFolderPath(string baseName)
    {
        return PathConstants.AdventureSubConfigurationFolderPath + "/" + baseName + "Configuration";
    }

    public string GetConfigurationFolderPath()
    {
        return PathConstants.AdventureConfigurationFolderPath;
    }

    public string GetGameConfigurationManagerMethodTemplatePath()
    {
        return PathConstants.AdventureGameConfigurationManagerMethodTemplatePath;
    }

    public string GetGameConfigurationManagerPath()
    {
        return PathConstants.AdventureGameConfigurationManagerPath;
    }

    public CodeNamespace GetNamespace()
    {
        return new CodeNamespace(typeof(AdventureEventsManager).Namespace);
    }

    public string GetConfigurationAssetMenuAttributeName(string baseName, string className)
    {
        return "Configuration/AdventureGame/" + baseName + "Configuration/" + className;
    }

    public Type GetConfigurationType()
    {
        return typeof(AdventureGameConfiguration);
    }

    public Type GetEditorConfigurationsType()
    {
        return typeof(AdventureGameConfigurations);
    }
}