using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkSceneCreation : CreationModuleComponent
    {
        public SceneAsset CreatedSceneAsset;
        public LevelChunkSceneCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
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

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, LevelChunkPrefabCreation levelChunkPrefabCreation, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var scenePath = editorInformationsData.InstancePath.LevelChunkScenePath + "/" + editorInformationsData.LevelZoneChunkID.ToString() + "_Chunk.unity";
            if (EditorSceneManager.SaveScene(createdScene, scenePath))
            {
                PrefabUtility.InstantiatePrefab(levelChunkPrefabCreation.CreatedPrefab);
                EditorSceneManager.SaveScene(createdScene, scenePath);
                this.CreatedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                editorProfile.AddToGeneratedObjects(new Object[] { this.CreatedSceneAsset });

            }
        }
    }
}