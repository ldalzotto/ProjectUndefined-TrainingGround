using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using UnityEngine.EventSystems;
using CreationWizard;
using System.Collections.Generic;

namespace Editor_PuzzleGameCreationWizard
{
    public class EditorInformationsHelper
    {
        public static void InitProperties(ref CommonGameConfigurations CommonGameConfigurations)
        {
            #region Puzzle Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, "_GameManagers_Persistance_Instanciater");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, "CorePuzzleSceneElements");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.EventSystem, "EventSystem");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.PuzzleDebugModule, "PuzzleDebugModule");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, "BasePuzzleLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab, "BaseLevelprefab");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab, "BaseAIPrefab");
            #endregion

            #region Game Configuration
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration, "t:" + typeof(LevelConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration, "t:" + typeof(LevelZonesSceneConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration, "t:" + typeof(LevelHierarchyConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, "t:" + typeof(ChunkZonesSceneConfiguration).Name);
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration, "t:" + typeof(AIComponentsConfiguration).Name);
            #endregion
        }

        public static string ComputeErrorState(ref CommonGameConfigurations CommonGameConfigurations)
        {
            return new List<string>()
            {
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration, nameof(CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration)),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration, nameof(CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration)),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration, nameof(CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration)),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, nameof(CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration)),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration, nameof(CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration)),

                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance)),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements) ),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.EventSystem, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.EventSystem) ),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.PuzzleDebugModule, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.PuzzleDebugModule) ),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics) ),
                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab, nameof(CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab) ),

                ErrorHelper.NonNullity(CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab, nameof(CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab) )
            }
             .Find((s) => !string.IsNullOrEmpty(s));
            // ErrorHelper.NonNullity(X, nameof(X)),
        }
    }

    [System.Serializable]
    public class CommonGameConfigurations
    {
        public PuzzleGameConfigurations PuzzleGameConfigurations;
        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;
        public PuzzleAICommonPrefabs PuzzleAICommonPrefabs;
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
        [ReadOnly]
        public AIComponentsConfiguration AIComponentsConfiguration;
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
    public class PuzzleAICommonPrefabs
    {
        [ReadOnly]
        public NPCAIManager AIBasePrefab;
    }

    [System.Serializable]
    public class InstacePath
    {
        [ReadOnly]
        public string LevelScenePath = "Assets/_Scenes/RTPuzzles";
        [ReadOnly]
        public string LevelChunkScenePath = "Assets/_Scenes/RTPuzzles";
        [ReadOnly]
        public string LevelConfigurationDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/Level/LevelConfigurationData";
        [ReadOnly]
        public string LevelCompletionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/Level/CompletionConfiguration/Data";
        [ReadOnly]
        public string LevelCompletionConditionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/Level/CompletionCondition/Data";
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
        [ReadOnly]
        public string AIPrefabPaths = "Assets/~RTPuzzleGame/AI/Prefabs";
        [ReadOnly]
        public string AIBehaviorConfigurationPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/AIComponentsConfiguration/Configuration";
    }
}