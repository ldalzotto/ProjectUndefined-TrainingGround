using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public LevelChunkConfigurationCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => typeof(LevelChunkConfigurationCreation).Name;

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile, LevelHierarchyCreation levelHierarchyCreation)
        {
            var createdChunkConfig = this.CreateAsset(editorInformationsData.InstancePath.LevelZoneChunkSceneConfigurationDataPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.LevelChunkSceneConfigurationData);
            editorProfile.AddToGeneratedObjects(new Object[] { createdChunkConfig });

            editorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZoneChunkID, createdChunkConfig);
            editorProfile.GameConfigurationModified(editorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, editorInformationsData.LevelZoneChunkID, createdChunkConfig);

            levelHierarchyCreation.CreatedObject.LevelHierarchy = new System.Collections.Generic.List<LevelZoneChunkID>();
            levelHierarchyCreation.CreatedObject.LevelHierarchy.Add(editorInformationsData.LevelZoneChunkID);
        }

        public void AfterSceneCreation(EditorInformationsData editorInformationsData, LevelChunkSceneCreation levelChunkSceneCreation)
        {
            this.CreatedObject.scene = levelChunkSceneCreation.CreatedSceneAsset;
        }
    }
}