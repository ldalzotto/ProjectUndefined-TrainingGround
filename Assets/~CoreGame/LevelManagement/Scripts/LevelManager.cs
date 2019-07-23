using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class LevelManager : MonoBehaviour
    {

        private LevelType currentLevelType;

        [SerializeField]
        private LevelZonesID levelID;
        [SerializeField]
        private LevelZoneChunkID currentLevelZoneChunkID = LevelZoneChunkID.NONE;
        [SerializeField]
        private List<LevelZoneChunkID> allLoadedLevelZonesChunkID;

        public LevelType CurrentLevelType { get => currentLevelType; }
        public LevelZonesID LevelID { get => levelID; set => levelID = value; }
        public List<LevelZoneChunkID> AllLoadedLevelZonesChunkID { get => allLoadedLevelZonesChunkID; }
        public LevelZoneChunkID CurrentLevelZoneChunkID { get => currentLevelZoneChunkID; }

        #region External Dependencies
        private Coroutiner Coroutiner;
        #endregion

        #region Internal Managers
        private EnvironmentSceneLevelManager EnvironmentSceneLevelManager;
        #endregion

        public void Init(LevelType currentLevelType)
        {
            var LevelAvailabilityManager = GameObject.FindObjectOfType<LevelAvailabilityManager>();
            var CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            var LevelManagerEventManager = GameObject.FindObjectOfType<LevelManagerEventManager>();

            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.currentLevelType = currentLevelType;
            this.EnvironmentSceneLevelManager = new EnvironmentSceneLevelManager(LevelAvailabilityManager, this, CoreConfigurationManager, LevelManagerEventManager);
        }

        #region External Event
        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            return this.EnvironmentSceneLevelManager.OnAdventureToPuzzleLevel(nextPuzzleLevel);
        }
        internal List<AsyncOperation> OnPuzzleToAdventureLevel(LevelZonesID nextPuzzleLevel)
        {
            return this.EnvironmentSceneLevelManager.OnPuzzleToAdventureLevel(nextPuzzleLevel);
        }
        public void OnChunkLevelEnter(LevelChunkType NextLevelChunk)
        {
            Debug.Log(MyLog.Format("LevelManager OnChunkLevelEnter"));
            this.currentLevelZoneChunkID = NextLevelChunk.LevelZoneChunkID;
        }

        public void OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            if (this.allLoadedLevelZonesChunkID == null) { this.allLoadedLevelZonesChunkID = new List<LevelZoneChunkID>(); }
            this.allLoadedLevelZonesChunkID.Add(levelZoneChunkID);
        }
        #endregion


        #region Data Retrieval
        public LevelZonesID GetCurrentLevel()
        {
            return levelID;
        }
        #endregion
    }

    class EnvironmentSceneLevelManager
    {

        #region External Dependencies
        private LevelAvailabilityManager LevelAvailabilityManager;
        private LevelManager LevelManagerRef;
        private LevelManagerEventManager LevelManagerEventManager;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        public EnvironmentSceneLevelManager(LevelAvailabilityManager levelAvailabilityManager, LevelManager LevelManagerRef, CoreConfigurationManager CoreConfigurationManager, LevelManagerEventManager LevelManagerEventManager)
        {
            LevelAvailabilityManager = levelAvailabilityManager;
            this.LevelManagerRef = LevelManagerRef;
            this.CoreConfigurationManager = CoreConfigurationManager;
            this.LevelManagerEventManager = LevelManagerEventManager;

            LoadAllLevels(this.LevelManagerRef.GetCurrentLevel(), async: false);
        }

        private List<AsyncOperation> LoadAllLevels(LevelZonesID levelZonesID, bool async = true)
        {
            List<AsyncOperation> sceneLoadOperations = new List<AsyncOperation>();
            foreach (var levelChunk in this.CoreConfigurationManager.LevelHierarchyConfiguration().GetLevelHierarchy(levelZonesID))
            {
                if (this.LevelAvailabilityManager.IsLevelAvailable(levelChunk))
                {
                    var sceneLoadAsyncOperation = SceneLoadingHelper.SceneLoadWithoutDuplicates(this.CoreConfigurationManager.ChunkZonesSceneConfiguration().GetSceneName(levelChunk), async);
                    if (sceneLoadAsyncOperation != null)
                    {
                        sceneLoadAsyncOperation.completed += (asyncOperation) => { this.LevelManagerEventManager.CORE_EVT_OnLevelChunkLoaded(levelChunk); };
                        sceneLoadOperations.Add(sceneLoadAsyncOperation);
                    }
                    else if (!async)
                    {
                        this.LevelManagerEventManager.CORE_EVT_OnLevelChunkLoaded(levelChunk);
                    }
                }
            }
            return sceneLoadOperations;
        }

        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            List<AsyncOperation> sceneUnloadOperations = new List<AsyncOperation>();
            foreach (var referenceChunk in this.CoreConfigurationManager.LevelHierarchyConfiguration().GetLevelHierarchy(LevelManagerRef.GetCurrentLevel()))
            {
                if (!this.CoreConfigurationManager.LevelHierarchyConfiguration().GetLevelHierarchy(nextPuzzleLevel).Contains(referenceChunk))
                {
                    var unloadAsyncOperation = this.SceneUnloadIfNecessary(referenceChunk);
                    if (unloadAsyncOperation != null)
                    {
                        sceneUnloadOperations.Add(unloadAsyncOperation);
                    }
                }
            }
            return sceneUnloadOperations;
        }
        internal List<AsyncOperation> OnPuzzleToAdventureLevel(LevelZonesID nextPuzzleLevel)
        {
            return this.LoadAllLevels(nextPuzzleLevel);
        }

        private AsyncOperation SceneUnloadIfNecessary(LevelZoneChunkID levelChunk)
        {
            return SceneLoadingHelper.SceneUnLoadWIthoutDuplicates(this.CoreConfigurationManager.ChunkZonesSceneConfiguration().GetSceneName(levelChunk));
        }


    }

    public enum LevelType
    {
        ADVENTURE, PUZZLE
    }



}
