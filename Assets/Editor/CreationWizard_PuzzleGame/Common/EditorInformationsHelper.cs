using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using UnityEngine.EventSystems;
using CreationWizard;
using System.Collections.Generic;
using ConfigurationEditor;
using System.Linq;

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
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseTargetZonePrefab, "TargetZoneBasePrefab");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab, "BaseAIPrefab");
            #endregion

            foreach (var configurationFieldInfo in CommonGameConfigurations.PuzzleGameConfigurations.GetType().GetFields())
            {
                var configurationObject = (Object)configurationFieldInfo.GetValue(CommonGameConfigurations.PuzzleGameConfigurations);
                if (configurationObject == null)
                {
                    AssetFinder.SafeSingleAssetFind(ref configurationObject, "t:" + configurationFieldInfo.FieldType.Name);
                    configurationFieldInfo.SetValue(CommonGameConfigurations.PuzzleGameConfigurations, configurationObject);
                }
            }
        }

        public static string ComputeErrorState(ref CommonGameConfigurations CommonGameConfigurations)
        {

            return NonNullityFieldCheck(CommonGameConfigurations.PuzzleGameConfigurations)
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.PuzzleLevelCommonPrefabs)).ToList()
                   .Find((s) => !string.IsNullOrEmpty(s));
        }

        static List<string> NonNullityFieldCheck(object containerObjectToCheck)
        {
            return containerObjectToCheck.GetType().GetFields().ToList().ConvertAll(field =>
             {
                 return ErrorHelper.NonNullity((Object)field.GetValue(containerObjectToCheck), field.Name);
             });
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
        [ReadOnly]
        public TargetZonesConfiguration TargetZonesConfiguration;
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
        [ReadOnly]
        public TargetZone BaseTargetZonePrefab;
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
        [ReadOnly]
        public string TargetZoneConfigurationDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/TargetZonesConfiguration/Data";
        [ReadOnly]
        public string TargetZonePrefabPath = "Assets/~RTPuzzleGame/TargetZone/Prefab";
    }
}