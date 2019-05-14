using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace CoreGame
{
    public class LevelManager : MonoBehaviour
    {

        private LevelType currentLevelType;

        [SerializeField]
        private LevelZonesID levelID;

        public LevelType CurrentLevelType { get => currentLevelType; }

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

            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.currentLevelType = currentLevelType;
            this.EnvironmentSceneLevelManager = new EnvironmentSceneLevelManager(LevelAvailabilityManager, this, CoreConfigurationManager);
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
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        public EnvironmentSceneLevelManager(LevelAvailabilityManager levelAvailabilityManager, LevelManager LevelManagerRef, CoreConfigurationManager CoreConfigurationManager)
        {
            LevelAvailabilityManager = levelAvailabilityManager;
            this.LevelManagerRef = LevelManagerRef;
            this.CoreConfigurationManager = CoreConfigurationManager;

            LoadAllLevels(this.LevelManagerRef.GetCurrentLevel());
        }

        private List<AsyncOperation> LoadAllLevels(LevelZonesID levelZonesID)
        {
            List<AsyncOperation> sceneLoadOperations = new List<AsyncOperation>();
            foreach (var levelChunk in this.CoreConfigurationManager.LevelZonesSceneConfiguration().GetLevelHierarchy(levelZonesID))
            {
                var sceneLoadAsyncOperation = SceneLoadingHelper.SceneLoadWithoutDuplicates(this.CoreConfigurationManager.ChunkZonesSceneConfiguration().GetSceneName(levelChunk));
                if (sceneLoadAsyncOperation != null)
                {
                    sceneLoadOperations.Add(sceneLoadAsyncOperation);
                }
            }
            return sceneLoadOperations;
        }

        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            List<AsyncOperation> sceneUnloadOperations = new List<AsyncOperation>();
            foreach (var referenceChunk in this.CoreConfigurationManager.LevelZonesSceneConfiguration().GetLevelHierarchy(LevelManagerRef.GetCurrentLevel()))
            {
                if (!this.CoreConfigurationManager.LevelZonesSceneConfiguration().GetLevelHierarchy(nextPuzzleLevel).Contains(referenceChunk))
                {
                    var unloadAsyncOperation = this.SceneUnload(referenceChunk);
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

        private AsyncOperation SceneUnload(LevelZoneChunkID levelChunk)
        {
            return SceneManager.UnloadSceneAsync(this.CoreConfigurationManager.ChunkZonesSceneConfiguration().GetSceneName(levelChunk));
        }


    }

    public enum LevelType
    {
        ADVENTURE, PUZZLE
    }



}
