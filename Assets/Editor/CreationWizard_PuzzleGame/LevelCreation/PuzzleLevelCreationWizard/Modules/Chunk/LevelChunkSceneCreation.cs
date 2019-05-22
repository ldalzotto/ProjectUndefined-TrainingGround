using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkSceneCreation : CreateableSceneComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelChunkPrefabCreation = editorProfile.GetModule<LevelChunkPrefabCreation>();
            this.CreateNewScene();
            var scenePath = editorInformationsData.CommonGameConfigurations.InstancePath.LevelChunkScenePath + "/" + editorInformationsData.LevelZoneChunkID.ToString() + "_Chunk.unity";
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(levelChunkPrefabCreation.CreatedPrefab);
                this.SaveScene(scenePath);

                editorProfile.AddToGeneratedObjects(new Object[] { this.CreatedSceneAsset });

            }
        }
    }
}