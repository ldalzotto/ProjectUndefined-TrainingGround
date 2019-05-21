using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CreationWizard;
using RTPuzzle;
using System.Linq;
using System;
using CoreGame;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        [SerializeField]
        public EditorInformationsData EditorInformationsData;

        public EditorInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string headerDescriptionLabel => "Base informations used by the creation wizard.";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        private void InitProperties()
        {
            #region Puzzle Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, "_GameManagers_Persistance_Instanciater");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, "CorePuzzleSceneElements");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem, "EventSystem");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, "PuzzleDebugModule");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, "BasePuzzleLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab, "BaseLevelprefab");
            #endregion

            #region Game Configuration
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration, "t:" + typeof(LevelConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration, "t:" + typeof(LevelZonesSceneConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration, "t:" + typeof(LevelHierarchyConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, "t:" + typeof(ChunkZonesSceneConfiguration).Name);
            #endregion
        }

        public override string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return new List<string>()
            {
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration, nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration)),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration, nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration)),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration, nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration)),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, nameof(this.EditorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration)),

                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance)),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab) )
            }
            .Find((s) => !string.IsNullOrEmpty(s));

            // ErrorHelper.NonNullity(X, nameof(X)),
        }

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return new List<string>() {
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration)),
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration)),
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration)),
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.LevelZoneChunkID,
                        this.EditorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.PuzzleGameConfigurations.ChunkZonesSceneConfiguration))

            }
            .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public PuzzleGameConfigurations PuzzleGameConfigurations;
        [SearchableEnum]
        public LevelZonesID LevelZonesID;
        [SearchableEnum]
        public LevelZoneChunkID LevelZoneChunkID;

        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;
        public InstacePath InstancePath;
    }

    [System.Serializable]
    public class PuzzleGameConfigurations
    {
        [ReadOnly]
        public LevelConfiguration LevelConfiguration;
        [ReadOnly]
        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration;
        [ReadOnly]
        public LevelHierarchyConfiguration LevelHierarchyConfiguration;
        [ReadOnly]
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration;
    }

    [System.Serializable]
    public class PuzzleLevelCommonPrefabs
    {
        [ReadOnly]
        public GameManagerPersistanceInstance GameManagerPersistanceInstance;
        [ReadOnly]
        public GameObject CorePuzzleSceneElements;
        [ReadOnly]
        public EventSystem EventSystem;
        [ReadOnly]
        public PuzzleDebugModule PuzzleDebugModule;
        [ReadOnly]
        public LevelManager BasePuzzleLevelDynamics;
        [ReadOnly]
        public LevelChunkType BaseLevelChunkPrefab;
    }

    [System.Serializable]
    public class InstacePath
    {
        [ReadOnly]
        public string LevelScenePath = "Assets/_Scenes/RTPuzzles";
        [ReadOnly]
        public string LevelChunkScenePath = "Assets/_Scenes/RTPuzzles";
        [ReadOnly]
        public string LevelConfigurationDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelConfigurationData";
        [ReadOnly]
        public string LevelCompletionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelCompletionConfiguration/LevelCompletionConfigurationData";
        [ReadOnly]
        public string LevelCompletionConditionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelCompletionCondition/ConfigurationData";
        [ReadOnly]
        public string PuzzleLevelDynamicsPath = "Assets/~CoreGame/LevelManagement/Prefab";
        [ReadOnly]
        public string PuzzleLevelHierarchyDataPath = "Assets/~CoreGame/Configuration/SubConfigurations/LevelHierarchyConfiguration/LevelHierarchyConfigurationData";
        [ReadOnly]
        public string LevelZoneSceneConfigurationDataPath = "Assets/~CoreGame/Configuration/SubConfigurations/LevelZonesSceneConfiguration/LevelZonesSceneConfigurationData";
        [ReadOnly]
        public string LevelZoneChunkSceneConfigurationDataPath = "Assets/~CoreGame/Configuration/SubConfigurations/ChunkZonesSceneConfiguration/ChunkZonesSceneConfigurationData";
        [ReadOnly]
        public string LevelChunkBaseLevelPrefabPath = "Assets/~CoreGame/LevelManagement/Prefab";
    }

}
