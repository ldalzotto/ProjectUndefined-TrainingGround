﻿using CoreGame;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using UnityEditor;

[System.Serializable]
public abstract class ALevelSceneConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
{

    protected abstract LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile);
    protected abstract CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile);
    protected abstract SceneAsset GetCreatedSceneAsset(AbstractCreationWizardEditorProfile editorProfile);

    public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
    {
        var levelZoneConfiguration = this.CreateAsset(GetCommonGameConfigurations(editorProfile).InstancePath.LevelZoneSceneConfigurationDataPath, GetLevelZonesID(editorProfile) + NameConstants.LevelSceneConfigurationData, editorProfile);

        GetCommonGameConfigurations(editorProfile).PuzzleGameConfigurations.LevelZonesSceneConfiguration.SetEntry(GetLevelZonesID(editorProfile), levelZoneConfiguration);
        editorProfile.GameConfigurationModified(GetCommonGameConfigurations(editorProfile).PuzzleGameConfigurations.LevelZonesSceneConfiguration, GetLevelZonesID(editorProfile), levelZoneConfiguration);
    }

    public override void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile)
    {
        SerializableObjectHelper.Modify(this.CreatedObject, (so) => { so.FindProperty(nameof(this.CreatedObject.scene)).objectReferenceValue = this.GetCreatedSceneAsset(editorProfile); });
    }

}