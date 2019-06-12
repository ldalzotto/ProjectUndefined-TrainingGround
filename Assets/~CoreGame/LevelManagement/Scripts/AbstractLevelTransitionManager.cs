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
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion
        private bool isNewZoneLoading;

        public virtual void Init()
        {
            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
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
            if (LevelChangeType == LevelChangeType.ADVENTURE_TO_PUZZLE || LevelChangeType == LevelChangeType.PUZZLE_TO_PUZZLE )
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
            SceneManager.UnloadSceneAsync(this.CoreConfigurationManager.LevelZonesSceneConfiguration().GetSceneName(LevelManager.GetCurrentLevel()));
            var nextZoneSceneName = this.CoreConfigurationManager.LevelZonesSceneConfiguration().GetSceneName(nextZone);
            SceneManager.LoadScene(nextZoneSceneName, LoadSceneMode.Additive);
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextZoneSceneName));
        }

        protected abstract void OnLevelChange_IMPL();
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
