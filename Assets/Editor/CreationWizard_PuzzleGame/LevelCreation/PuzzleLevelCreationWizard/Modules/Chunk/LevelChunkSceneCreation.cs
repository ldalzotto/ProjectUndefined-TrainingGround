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
        public LevelChunkSceneCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, LevelChunkPrefabCreation levelChunkPrefabCreation, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.CreateNewScene();
            var scenePath = editorInformationsData.InstancePath.LevelChunkScenePath + "/" + editorInformationsData.LevelZoneChunkID.ToString() + "_Chunk.unity";
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(levelChunkPrefabCreation.CreatedPrefab);
                this.SaveScene(scenePath);

                editorProfile.AddToGeneratedObjects(new Object[] { this.CreatedSceneAsset });

            }
        }
    }
}