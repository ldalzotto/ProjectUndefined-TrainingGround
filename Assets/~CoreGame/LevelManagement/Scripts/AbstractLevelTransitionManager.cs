using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
            if (LevelChangeType == LevelChangeType.ADVENTURE_TO_PUZZLE)
            {
                this.LevelManager.OnAdventureToPuzzleLevel(nextZone);
            }
            else
            {
                this.LevelManager.OnPuzzleToAdventureLevel(nextZone);
            }
            SceneManager.UnloadSceneAsync(LevelZones.LevelZonesSceneName[LevelManager.GetCurrentLevel()]).completed += (AsyncOperation asyncOperation) =>
            {
                SceneLoadHelper.LoadSceneAsync(Coroutiner, nextZone, LoadSceneMode.Additive, (AsyncOperation) =>
                {
                    isNewZoneLoading = false;
                });
            };
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
