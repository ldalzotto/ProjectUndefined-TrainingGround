using AdventureGame;
using RTPuzzle;
using System.CodeDom;

public static class PathConstants
{
    public const string AIComponentBasePath = "Assets/~RTPuzzleGame/AI/AIComponents/Scripts";
    public const string GenericPuzzleAIComponentsFilePath = "Assets/~RTPuzzleGame/AI/AIComponents/AIComponentContainer/Scripts/GenericPuzzleAIComponents.cs";
    public const string AIModuleWizardConstant = "Assets/Editor/GameDesigner/Modules/AI/AIModuleWizard/AIManagerModuleWizardConstants.cs";
    public const string AICommonPrefabsPath = "Assets/Editor/CreationWizard_PuzzleGame/Common/PuzzleAICommonPrefabs.cs";
    public const string EditorCreationWizardFolderPath = "Assets/Editor/CreationWizard_PuzzleGame/ObjectsCreation";
    public const string InteractiveObjectModulePath = "Assets/~RTPuzzleGame/InteractiveObject/Modules";
    public const string INteractiveObjectStaticConfigurationPath = "Assets/~RTPuzzleGame/InteractiveObject/Script/StaticConfiguration";

    public const string PointOfInterestModulePath = "Assets/~AdventureGame/PointOfInterest/ScenePOI/Modules";
    public const string PointOfInterestDefinitionPath = "Assets/~AdventureGame/Configuration/SubConfiguration/PointOfInterestDefinitionConfiguration/PointOfInterestDefinition";

    public const string PuzzleGameConfigurationsEditorConstantsPath = "Assets/Editor/CreationWizard_PuzzleGame/Common";

    public const string GameDesignerBasePath = "Assets/Editor/GameDesigner";
    public const string GameDesignerModulesPath = "Assets/Editor/GameDesigner/Modules";
    public const string CustomEditorPath = "Assets/Editor/GameCustomEditors";
    public const string InteractiveObjectInitializationObjectPath = "Assets/~RTPuzzleGame/InteractiveObject/Script/InteractiveObjectInitializationObject.cs";
    public const string IDBasePath = "Assets/@GameConfigurationID/IDs";
    public const string PuzzleConfigurationFolderPath = "Assets/~RTPuzzleGame/Configuration";
    public const string AdventureConfigurationFolderPath = "Assets/~AdventureGame/Configuration";
    public const string PuzzleSubConfigurationFolderPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration";
    public const string AdventureSubConfigurationFolderPath = "Assets/~AdventureGame/Configuration/SubConfiguration";
    public const string PuzzleGameConfigurationsEditorConstantsPath2 = "Assets/Editor/CreationWizard_PuzzleGame/Constants";
    public const string PuzzleGameConfigurationsEditorPath = "Assets/Editor/CreationWizard_PuzzleGame";
    public const string GameDesignerConfigurationModulesPath = "Assets/Editor/GameDesigner/Modules/Configurations/ConfigurationsModule.cs";

    public const string PuzzleGameConfigurationManagerPath = "Assets/~RTPuzzleGame/Configuration/PuzzleGameConfigurationManager.cs";
    public const string AdventureGameConfigurationManagerPath = "Assets/~AdventureGame/Configuration/AdventureGameConfigurationManager.cs";

    public const string InteractiveObjectModuleDefinitionPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/InteractiveObjectTypeDefinitionConfiguration/InteractiveObjectDefinition";
    public const string GameDesignerChoiceTreeConstantPath = "Assets/Editor/GameDesigner/ChoiceTree/ChoiceTreeConstant.cs";

    public const string CodeGenerationCreationWizardBasincConfigurationCreationTemplatePath = "Assets/Editor/CodeGeneration/Templates/CreationWizardBasicConfigurationCreation";
    public const string InteractiveObjectIdentifiedModuleWizardConfigurationTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleWizardConfiguration/InteractiveObjetIdentifiedModuleWizardConfigurationTemplate.txt";
    public const string InteractiveObjectNonIdentifiedModuleWizardConfigurationTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleWizardConfiguration/InteractiveObjetNonIdentifiedModuleWizardConfigurationTemplate.txt";
    public const string IdentifiedInteractiveObjectModulesInitializationOperationsMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModulesInitializationOperations/IdentifiedInteractiveObjectModulesInitializationOperationsMethodTemplate.txt";
    public const string NonIdentifiedInteractiveObjectModulesInitializationOperationsMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModulesInitializationOperations/NonIdentifiedInteractiveObjectModulesInitializationOperationsMethodTemplate.txt";
    public const string CustomEditorTemplatePath = "Assets/Editor/CodeGeneration/Templates/CustomEditorTemplate/${baseName}CustomEditor.cs.txt";
    public const string CodeGenrationGameDesignerConfigurationCreationTemplatePath = "Assets/Editor/CodeGeneration/Templates/GameDesignerCreationModule";
    public const string GameDesignerConfigurationModuleTemplatepath = "Assets/Editor/CodeGeneration/Templates/GameDesignerTemplates/GameDesignerConfigurationModuleTemplate.txt";
    public const string PuzzleGameConfigurationManagerMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/GameConfiguration/PuzzleGameConfigurationManagerMethodTemplate.txt";
    public const string AdventureGameConfigurationManagerMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/GameConfiguration/AdventureGameConfigurationManagerMethodTemplate.txt";
    
    public const string InteractiveObjectDefinitionConditionTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleDefinition/DefinitionCondition.txt";
    public const string InteractiveObjectDefinitionCustomEditorCondition = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleDefinition/DefinitionCustomEditorCondition.txt";


    public const string PointOfInterestObjectDefinitionConditionTemplatePath = "Assets/Editor/CodeGeneration/Templates/PointOfInterestModuleDefinition/DefinitionCondition.txt";
    public const string PointOfInterestObjectDefinitionCustomEditorCondition = "Assets/Editor/CodeGeneration/Templates/PointOfInterestModuleDefinition/DefinitionCustomEditorCondition.txt";
}
