using CoreGame;
using Editor_MainGameCreationWizard;
using GameConfigurationID;

[System.Serializable]
public abstract class ALevelHierarchyCreation : CreateableScriptableObjectComponent<LevelHierarchyConfigurationData>
{

    protected abstract LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile);
    protected abstract CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile);

    public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
    {
        var generatedHierarchy = this.CreateAsset(this.GetCommonGameConfigurations(editorProfile).InstancePath.PuzzleLevelHierarchyDataPath, this.GetLevelZonesID(editorProfile) + NameConstants.LevelHierarchyConfigurationData, editorProfile);

        this.GetCommonGameConfigurations(editorProfile).PuzzleGameConfigurations.LevelHierarchyConfiguration.SetEntry(this.GetLevelZonesID(editorProfile), generatedHierarchy);
        editorProfile.GameConfigurationModified(this.GetCommonGameConfigurations(editorProfile).PuzzleGameConfigurations.LevelHierarchyConfiguration, this.GetLevelZonesID(editorProfile), generatedHierarchy);
    }
}