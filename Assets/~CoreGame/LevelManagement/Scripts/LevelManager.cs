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
        public void OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            this.EnvironmentSceneLevelManager.OnAdventureToPuzzleLevel(nextPuzzleLevel);
        }
        internal void OnPuzzleToAdventureLevel(LevelZonesID nextPuzzleLevel)
        {
            this.EnvironmentSceneLevelManager.OnPuzzleToAdventureLevel(nextPuzzleLevel);
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
        private List<LevelZoneChunkID> LoadedEnvironmentLevels = new List<LevelZoneChunkID>();

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

        private void LoadAllLevels(LevelZonesID levelZonesID)
        {
            if (LevelZones.LevelHierarchy.ContainsKey(levelZonesID))
            {
                foreach (var level in LevelZones.LevelHierarchy[levelZonesID])
                {
                    this.SceneLoadWithoutDuplicates(level);
                }
            }
        }

        public void OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            foreach (var loadedChunk in this.LoadedEnvironmentLevels)
            {
                if (!LevelZones.LevelHierarchy[nextPuzzleLevel].Contains(loadedChunk))
                {
                    this.SceneUnload(loadedChunk);
                }
            }
        }
        internal void OnPuzzleToAdventureLevel(LevelZonesID nextPuzzleLevel)
        {
            this.LoadAllLevels(nextPuzzleLevel);
        }

        private void SceneLoadWithoutDuplicates(LevelZoneChunkID levelChunk)
        {
            var name = LevelZones.LevelZonesChunkScenename[levelChunk];
            if (!SceneManager.GetSceneByName(name).isLoaded && !this.LoadedEnvironmentLevels.Contains(levelChunk))
            {
                SceneManager.LoadScene(name, LoadSceneMode.Additive);
                this.LoadedEnvironmentLevels.Add(levelChunk);
            }
        }

        private void SceneUnload(LevelZoneChunkID levelChunk)
        {
            if (this.LoadedEnvironmentLevels.Contains(levelChunk))
            {
                SceneManager.UnloadSceneAsync(LevelZones.LevelZonesChunkScenename[levelChunk]);
            }
        }


    }

    public enum LevelType
    {
        ADVENTURE, PUZZLE
    }



}
