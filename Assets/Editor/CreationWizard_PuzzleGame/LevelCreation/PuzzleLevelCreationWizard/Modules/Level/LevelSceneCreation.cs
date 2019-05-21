using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelSceneCreation : CreationModuleComponent
    {
        public SceneAsset CreatedSceneAsset;

        public LevelSceneCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
            this.CreatedSceneAsset = null;
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.CreatedSceneAsset)));
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, PuzzleLevelDynamicsCreation puzzleLevelDynamicsCreation,
            Action<UnityEngine.Object[]> addToGenerated)
        {
            var createdScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var scenePath = editorInformationsData.InstancePath.LevelScenePath + "/" + editorInformationsData.LevelZonesID.ToString() + ".unity";
            if (EditorSceneManager.SaveScene(createdScene, scenePath))
            {
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.EventSystem);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance);
                PrefabUtility.InstantiatePrefab(editorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule);
                PrefabUtility.InstantiatePrefab(puzzleLevelDynamicsCreation.CreatedPrefab);

                EditorSceneManager.SaveScene(createdScene, scenePath);

                this.CreatedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                addToGenerated.Invoke(new UnityEngine.Object[] { this.CreatedSceneAsset });
            }
        }
    }


}
