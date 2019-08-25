using AdventureGame;
using CoreGame;
using CreationWizard;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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

            #region Puzzle Interactive Object Modules Prefabs
            foreach (var configurationFieldInfo in CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.GetType().GetFields())
            {
                var configurationObject = (Object)configurationFieldInfo.GetValue(CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs);
                if (configurationObject == null)
                {

                    foreach (var foundAsset in AssetFinder.SafeAssetFind(configurationFieldInfo.FieldType.Name))
                    {
                        if (foundAsset.GetType() == typeof(GameObject))
                        {
                            var retrievedComp = ((GameObject)foundAsset).GetComponent(configurationFieldInfo.FieldType);
                            if (typeof(InteractiveObjectModule).IsAssignableFrom(retrievedComp.GetType()))
                            {
                                configurationFieldInfo.SetValue(CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs, retrievedComp);
                            }
                        }
                    }
                }
            }
            #endregion
            
            #region Adventure Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BasePOIPrefab, "BasePOIPrefab");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BaseAdventureLevelDynamics, "BaseAdventureLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.CommonAdventureObjects, "CommonAdventureObjects");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.AdventureGameManagersNonPersistant, "_AdventureGameManagersNonPersistant");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.AdventureCommonPrefabs.BasePointOfInterestTrackerModule, "BasePOITrackerModule");
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

            foreach (var configurationFieldInfo in CommonGameConfigurations.AdventureGameConfigurations.GetType().GetFields())
            {
                var configurationObject = (Object)configurationFieldInfo.GetValue(CommonGameConfigurations.AdventureGameConfigurations);
                if (configurationObject == null)
                {
                    AssetFinder.SafeSingleAssetFind(ref configurationObject, "t:" + configurationFieldInfo.FieldType.Name);
                    configurationFieldInfo.SetValue(CommonGameConfigurations.AdventureGameConfigurations, configurationObject);
                }
            }

            foreach (var configurationFieldInfo in CommonGameConfigurations.CoreGameConfigurations.GetType().GetFields())
            {
                var configurationObject = (Object)configurationFieldInfo.GetValue(CommonGameConfigurations.CoreGameConfigurations);
                if (configurationObject == null)
                {
                    AssetFinder.SafeSingleAssetFind(ref configurationObject, "t:" + configurationFieldInfo.FieldType.Name);
                    configurationFieldInfo.SetValue(CommonGameConfigurations.CoreGameConfigurations, configurationObject);
                }
            }

        }

        public static string ComputeErrorState(ref CommonGameConfigurations CommonGameConfigurations)
        {

            return NonNullityFieldCheck(CommonGameConfigurations.PuzzleGameConfigurations)
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.AdventureGameConfigurations))
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.CoreGameConfigurations))
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.PuzzleLevelCommonPrefabs))
                        .Concat(NonNullityFieldCheck(CommonGameConfigurations.AdventureCommonPrefabs))
                        .ToList()
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
        public CoreGameConfigurations CoreGameConfigurations;

        public AdventureGameConfigurations AdventureGameConfigurations;
        public AdventureCommonPrefabs AdventureCommonPrefabs;

        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;
        public PuzzleGameConfigurations PuzzleGameConfigurations;
        public PuzzleInteractiveObjectModulePrefabs PuzzleInteractiveObjectModulePrefabs;
        
        public CommonGameConfigurations()
        {
            this.CoreGameConfigurations = new CoreGameConfigurations();
            this.PuzzleGameConfigurations = new PuzzleGameConfigurations();
            this.AdventureGameConfigurations = new AdventureGameConfigurations();
            this.PuzzleLevelCommonPrefabs = new PuzzleLevelCommonPrefabs();
            this.AdventureCommonPrefabs = new AdventureCommonPrefabs();
            this.PuzzleInteractiveObjectModulePrefabs = new PuzzleInteractiveObjectModulePrefabs();
        }
    }

    [System.Serializable]
    public class CoreGameConfigurations
    {
        [ReadOnly]
        public AnimationConfiguration AnimationConfiguration;
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
    public class AdventureGameConfigurations
    {
        [ReadOnly]
        public PointOfInterestConfiguration PointOfInterestConfiguration;
        [ReadOnly]
        public DiscussionTreeConfiguration DiscussionTreeConfiguration;
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