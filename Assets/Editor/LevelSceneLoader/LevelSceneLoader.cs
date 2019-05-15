using UnityEngine;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using CoreGame;
using UnityEngine.SceneManagement;

namespace Editor_LevelSceneLoader
{
    public class LevelSceneLoader : EditorWindow
    {
        [MenuItem("Level/SceneLoad")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            LevelSceneLoader window = (LevelSceneLoader)EditorWindow.GetWindow(typeof(LevelSceneLoader));
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("LOAD"))
            {
                var levelManager = GameObject.FindObjectOfType<LevelManager>();
                if (levelManager == null)
                {
                    Debug.LogError("No level manager found.");
                }
                else
                {
                    var levelHierarchyConfiguration = AssetFinder.SafeSingleAssetFind<LevelHierarchyConfiguration>("t:" + typeof(LevelHierarchyConfiguration).Name);
                    var chunkZonesConfiguration = AssetFinder.SafeSingleAssetFind<ChunkZonesSceneConfiguration>("t:" + typeof(ChunkZonesSceneConfiguration).Name);
                    if (chunkZonesConfiguration == null)
                    {
                        Debug.LogError("The chunk zone configuration has not been found.");
                    }
                    else
                    {
                        foreach (var chunkId in levelHierarchyConfiguration.GetLevelHierarchy(levelManager.GetCurrentLevel()))
                        {
                            this.SceneLoadWithoutDuplicate(chunkZonesConfiguration.GetSceneName(chunkId));
                        }
                    }
                }
            }
        }

        private void SceneLoadWithoutDuplicate(string sceneToLoadName)
        {
            var sceneNB = EditorSceneManager.sceneCount;
            bool load = true;
            for (var i = 0; i < sceneNB; i++)
            {
                if (EditorSceneManager.GetSceneAt(i).name == sceneToLoadName)
                {
                    load = false;
                }
            }
            if (load)
            {
                var scene = AssetFinder.SafeSingleAssetFind<SceneAsset>(sceneToLoadName + " t:Scene");
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
            }
        }

    }

}
