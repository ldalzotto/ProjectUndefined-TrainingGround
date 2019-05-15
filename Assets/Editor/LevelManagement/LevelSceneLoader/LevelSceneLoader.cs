using UnityEngine;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using CoreGame;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static VisualElementsHelper;

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

        private Button loadButton;
        private ScenesToLoadLayout ScenesToLoadLayout;

        private void OnEnable()
        {
            var root = this.rootVisualElement;

            var refreshButton = new Button(this.OnRefreshButtonClick);
            refreshButton.text = "Refresh scenes to load";
            root.Add(refreshButton);

            this.ScenesToLoadLayout = new ScenesToLoadLayout(root);

            this.loadButton = new Button(this.OnLoadButtonClick);
            loadButton.text = "LOAD";
            loadButton.SetEnabled(false);

            root.Add(loadButton);
        }

        private void OnLoadButtonClick()
        {
            foreach (var sceneToLoad in this.ScenesToLoadLayout.SceneToLoadLines)
            {
                if (sceneToLoad.IsElligible)
                {
                    this.SceneLoadWithoutDuplicate(sceneToLoad.Scene.name);
                }
            }
        }

        private List<SceneAsset> GetAllDependantChunckScene()
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
                    return levelHierarchyConfiguration.GetLevelHierarchy(levelManager.GetCurrentLevel()).ConvertAll((chunkId) =>
                    {
                        return chunkZonesConfiguration.ConfigurationInherentData[chunkId].scene;
                    });
                }
            }

            return new List<SceneAsset>();
        }

        private void OnRefreshButtonClick()
        {
            var allDependantChunckScenes = this.GetAllDependantChunckScene();
            this.ScenesToLoadLayout.Refresh(allDependantChunckScenes);
            if (allDependantChunckScenes != null && allDependantChunckScenes.Count > 0)
            {
                this.loadButton.SetEnabled(true);
            }
            else
            {
                this.loadButton.SetEnabled(false);
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

    public class ScenesToLoadLayout
    {
        private Box scenesToLoadContainerBox;
        private List<SceneToLoadLine> sceneToLoadLines = new List<SceneToLoadLine>();

        public ScenesToLoadLayout(VisualElement parent)
        {
            this.scenesToLoadContainerBox = new Box();
            parent.Add(this.scenesToLoadContainerBox);
        }

        public List<SceneToLoadLine> SceneToLoadLines { get => sceneToLoadLines; }

        public void Refresh(List<SceneAsset> lines)
        {
            this.scenesToLoadContainerBox.Clear();
            this.sceneToLoadLines.Clear();

            foreach (var line in lines)
            {
                this.LineLayout(line);
            }

            this.scenesToLoadContainerBox.MarkDirtyRepaint();
        }

        private void LineLayout(SceneAsset sceneToLoad)
        {
            var line = VisualElementWithStyle(new VisualElement(), (style) =>
            {
                style.flexDirection = FlexDirection.Row;
            });

            this.scenesToLoadContainerBox.Add(line);

            this.sceneToLoadLines.Add(new SceneToLoadLine(line, sceneToLoad));
        }

    }

    public class SceneToLoadLine
    {
        private bool isElligible = true;
        private SceneAsset scene;

        public SceneToLoadLine(VisualElement parent, SceneAsset sceneToLoad)
        {
            this.scene = sceneToLoad;
            var loadSceneToggle = new Toggle();
            loadSceneToggle.value = this.isElligible;
            loadSceneToggle.RegisterValueChangedCallback((changeEvent) =>
            {
                this.isElligible = changeEvent.newValue;
            });
            var sceneNameColumn = VisualElementWithStyle(new Label(sceneToLoad.name), (style) => { });

            parent.Add(loadSceneToggle);
            parent.Add(sceneNameColumn);
        }

        public bool IsElligible { get => isElligible; }
        public SceneAsset Scene { get => scene; }
    }

}
