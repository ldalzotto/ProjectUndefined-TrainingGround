﻿using GameConfigurationID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreGame
{
    public class LevelTransitionManager : MonoBehaviour
    {
        private bool isNewZoneLoading;

        #region External Events
        public void OnAdventureToPuzzleLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.ADVENTURE_TO_PUZZLE);
        }
        public void OnPuzzleToAdventureLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.PUZZLE_TO_ADVENTURE);
        }
        public void OnPuzzleToPuzzleLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.PUZZLE_TO_PUZZLE);
        }

        private void OnLevelChange(LevelZonesID nextZone, LevelChangeType LevelChangeType)
        {
            isNewZoneLoading = true;

            List<AsyncOperation> chunkOperations = null;
            if (LevelChangeType == LevelChangeType.ADVENTURE_TO_PUZZLE || LevelChangeType == LevelChangeType.PUZZLE_TO_PUZZLE)
            {
                chunkOperations = CoreGameSingletonInstances.LevelManagerEventManager.CORE_EVT_OnAdventureToPuzzleLevel(nextZone);
            }
            else
            {
                chunkOperations = CoreGameSingletonInstances.LevelManagerEventManager.CORE_EVT_OnPuzzleToAdventureLevel(nextZone);
            }

            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = false;
            }
            CoreGameSingletonInstances.Coroutiner.StopAllCoroutines();
            CoreGameSingletonInstances.Coroutiner.StartCoroutine(this.SceneTrasitionOperation(chunkOperations, nextZone));
        }

        private IEnumerator SceneTrasitionOperation(List<AsyncOperation> chunkOperations, LevelZonesID nextZone)
        {
            yield return new WaitForListOfAsyncOperation(chunkOperations);
            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = true;
            }
            isNewZoneLoading = false;
            SceneManager.UnloadSceneAsync(CoreGameSingletonInstances.CoreConfigurationManager.LevelZonesSceneConfiguration().GetSceneName(CoreGameSingletonInstances.LevelManager.GetCurrentLevel()));
            var nextZoneSceneName = CoreGameSingletonInstances.CoreConfigurationManager.LevelZonesSceneConfiguration().GetSceneName(nextZone);
            SceneManager.LoadScene(nextZoneSceneName, LoadSceneMode.Additive);
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextZoneSceneName));
        }
        #endregion

        #region Logical Conditions
        public bool IsNewZoneLoading() { return isNewZoneLoading; }
        #endregion

        enum LevelChangeType
        {
            PUZZLE_TO_ADVENTURE,
            ADVENTURE_TO_PUZZLE,
            PUZZLE_TO_PUZZLE
        }
    }


}
