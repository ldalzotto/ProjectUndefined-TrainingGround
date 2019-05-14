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
                    foreach (var chunkId in LevelZones.LevelHierarchy[levelManager.GetCurrentLevel()])
                    {
                        this.SceneLoadWithoutDuplicate(LevelZones.LevelZonesChunkScenename[chunkId]);
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
