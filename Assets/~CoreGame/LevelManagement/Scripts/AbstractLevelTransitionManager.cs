using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace CoreGame
{
    public abstract class AbstractLevelTransitionManager : MonoBehaviour
    {
        #region External Dependencies
        private Coroutiner Coroutiner;
        private LevelManager LevelManager;
        #endregion
        private bool isNewZoneLoading;

        public virtual void Init()
        {
            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
        }

        #region External Events
        public void OnAdventureToPuzzleLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.ADVENTURE_TO_PUZZLE);
        }
        public void OnPuzzleToAdventureLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.PUZZLE_TO_ADVENTURE);
        }

        private void OnLevelChange(LevelZonesID nextZone, LevelChangeType LevelChangeType)
        {
            isNewZoneLoading = true;
            this.OnLevelChange_IMPL();

            List<AsyncOperation> chunkOperations = null;
            if (LevelChangeType == LevelChangeType.ADVENTURE_TO_PUZZLE)
            {
                chunkOperations = this.LevelManager.OnAdventureToPuzzleLevel(nextZone);
            }
            else
            {
                chunkOperations = this.LevelManager.OnPuzzleToAdventureLevel(nextZone);
            }

            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = false;
            }
            this.Coroutiner.StopAllCoroutines();
            this.Coroutiner.StartCoroutine(this.SceneTrasitionOperation(chunkOperations, nextZone));
        }

        private IEnumerator SceneTrasitionOperation(List<AsyncOperation> chunkOperations, LevelZonesID nextZone)
        {
            yield return new WaitForListOfAsyncOperation(chunkOperations);
            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = true;
            }
            isNewZoneLoading = false;
            SceneManager.UnloadSceneAsync(LevelZones.LevelZonesSceneName[LevelManager.GetCurrentLevel()]);
            SceneLoadHelper.LoadScene(LoadSceneMode.Additive, nextZone);
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LevelZones.LevelZonesSceneName[nextZone]));
        }

        protected abstract void OnLevelChange_IMPL();
        #endregion

        #region Logical Conditions
        public bool IsNewZoneLoading() { return isNewZoneLoading; }
        #endregion

        enum LevelChangeType
        {
            PUZZLE_TO_ADVENTURE, ADVENTURE_TO_PUZZLE
        }
    }


}
