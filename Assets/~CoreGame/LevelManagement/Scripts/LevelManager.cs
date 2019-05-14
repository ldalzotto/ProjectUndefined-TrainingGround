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
            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.currentLevelType = currentLevelType;
            this.EnvironmentSceneLevelManager = new EnvironmentSceneLevelManager(LevelAvailabilityManager, this);
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
        #endregion

        public EnvironmentSceneLevelManager(LevelAvailabilityManager levelAvailabilityManager, LevelManager LevelManagerRef)
        {
            LevelAvailabilityManager = levelAvailabilityManager;
            this.LevelManagerRef = LevelManagerRef;

            LoadAllLevels(this.LevelManagerRef.GetCurrentLevel());
        }

        private List<AsyncOperation> LoadAllLevels(LevelZonesID levelZonesID)
        {
            List<AsyncOperation> sceneLoadOperations = new List<AsyncOperation>();
            if (LevelZones.LevelHierarchy.ContainsKey(levelZonesID))
            {
                foreach (var levelChunk in LevelZones.LevelHierarchy[levelZonesID])
                {
                    var sceneLoadAsyncOperation = SceneLoadingHelper.SceneLoadWithoutDuplicates(LevelZones.LevelZonesChunkScenename[levelChunk]);
                    if (sceneLoadAsyncOperation != null)
                    {
                        sceneLoadOperations.Add(sceneLoadAsyncOperation);
                    }
                }
            }
            return sceneLoadOperations;
        }

        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            List<AsyncOperation> sceneUnloadOperations = new List<AsyncOperation>();
            foreach (var referenceChunk in LevelZones.LevelHierarchy[LevelManagerRef.GetCurrentLevel()])
            {
                if (!LevelZones.LevelHierarchy[nextPuzzleLevel].Contains(referenceChunk))
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
            return SceneManager.UnloadSceneAsync(LevelZones.LevelZonesChunkScenename[levelChunk]);
        }


    }

    public enum LevelType
    {
        ADVENTURE, PUZZLE
    }



}
