using AdventureGame;
using ConfigurationEditor;
using CoreGame;
using CreationWizard;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Editor_MainGameCreationWizard
{
    public class EditorInformationsHelper
    {
        public static void InitProperties(ref CommonGameConfigurations CommonGameConfigurations)
        {
            #region Puzzle Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, "_GameManagers_Persistance_Instanciater");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, "CorePuzzleSceneElements");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.PuzzleDebugModule, "PuzzleDebugModule");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, "BasePuzzleLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab, "BaseLevelprefab");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseInteractiveObjectTypePrefab, "BaseInteractiveObjectPrefab");
            #endregion

            #region Adventure Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BasePOIPrefab, "BasePOIPrefab");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BaseAdventureLevelDynamics, "BaseAdventureLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.CommonAdventureObjects, "CommonAdventureObjects");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.AdventureGameManagersNonPersistant, "_AdventureGameManagersNonPersistant");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BasePointOfInterestTrackerModule, "BasePOITrackerModule");
            #endregion

            //TODO configuration initialization
            if (CommonGameConfigurations.Configurations.Count == 0)
            {
                foreach (var configurationType in TypeHelper.GetAllGameConfigurationTypes())
                {
                    CommonGameConfigurations.Configurations.Add(configurationType, (IConfigurationSerialization)AssetFinder.SafeAssetFind("t:" + configurationType.Name)[0]);
                }
            }
        }

        public static string ComputeErrorState(ref CommonGameConfigurations CommonGameConfigurations)
        {

            return NonNullityFieldCheck(NonNullityFieldCheck(CommonGameConfigurations.AdventureCommonPrefabs)
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.PuzzleLevelCommonPrefabs))
                        )
                        .ToList()
                   .Find((s) => !string.IsNullOrEmpty(s));
        }

        static List<string> NonNullityFieldCheck(object containerObjectToCheck)
        {
            return containerObjectToCheck.GetType().GetFields().ToList().ConvertAll(field =>
             {
                 return ErrorHelper.NonNullity((UnityEngine.Object)field.GetValue(containerObjectToCheck), field.Name);
             });
        }
    }

    [System.Serializable]
    public class CommonGameConfigurations
    {
        public AdventureCommonPrefabs AdventureCommonPrefabs;

        public Dictionary<Type, IConfigurationSerialization> Configurations;

        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;

        public T GetConfiguration<T>() where T : IConfigurationSerialization
        {
            return (T)this.Configurations[typeof(T)];
        }

        public IConfigurationSerialization GetConfiguration(Type configurationType)
        {
            return this.Configurations[configurationType];
        }

        public CommonGameConfigurations()
        {
            this.PuzzleLevelCommonPrefabs = new PuzzleLevelCommonPrefabs();
            this.AdventureCommonPrefabs = new AdventureCommonPrefabs();
            this.Configurations = new Dictionary<Type, IConfigurationSerialization>();
        }
    }


    [System.Serializable]
    public class PuzzleLevelCommonPrefabs
    {
        [ReadOnly]
        public GameManagerPersistanceInstance GameManagerPersistanceInstance;
        [ReadOnly]
        public GameObject CorePuzzleSceneElements;
        [ReadOnly]
        public PuzzleDebugModule PuzzleDebugModule;
        [ReadOnly]
        public LevelManager BasePuzzleLevelDynamics;
        [ReadOnly]
        public LevelChunkType BaseLevelChunkPrefab;
        [ReadOnly]
        public InteractiveObjectType BaseInteractiveObjectTypePrefab;
    }

    [System.Serializable]
    public class AdventureCommonPrefabs
    {
        [ReadOnly]
        public GameObject BasePOIPrefab;
        [ReadOnly]
        public LevelManager BaseAdventureLevelDynamics;
        [ReadOnly]
        public GameObject CommonAdventureObjects;
        [ReadOnly]
        public GameObject AdventureGameManagersNonPersistant;
        [ReadOnly]
        public PointOfInterestTrackerModule BasePointOfInterestTrackerModule;
    }
}