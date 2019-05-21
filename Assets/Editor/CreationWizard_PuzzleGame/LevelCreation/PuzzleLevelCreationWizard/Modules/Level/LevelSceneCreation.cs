using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelSceneCreation : CreateableSceneComponent
    {

        public LevelSceneCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }


        public void OnGenerationClicked(EditorInformationsData editorInformationsData, PuzzleLevelDynamicsCreation puzzleLevelDynamicsCreation,
            Action<UnityEngine.Object[]> addToGenerated)
        {
            this.CreateNewScene();
            var scenePath = editorInformationsData.InstancePath.LevelScenePath + "/" + editorInformationsData.LevelZonesID.ToString() + ".unity";
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.EventSystem);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule);
                PrefabUtility.InstantiatePrefab(puzzleLevelDynamicsCreation.CreatedPrefab);

                this.SaveScene(scenePath);
                addToGenerated.Invoke(new UnityEngine.Object[] { this.CreatedSceneAsset });
            }
        }
    }


}
