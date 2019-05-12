using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class AbstractLevelTransitionManager : MonoBehaviour
    {
        #region External Dependencies
        private Coroutiner Coroutiner;
        #endregion
        private bool isNewZoneLoading;

        public virtual void Init()
        {
            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
        }

        #region External Events
        public void OnLevelZoneChange(LevelZonesID nextZone)
        {
            isNewZoneLoading = true;
            this.OnLevelZoneChange_IMPL();
            SceneLoadHelper.LoadScene(Coroutiner, nextZone);
            isNewZoneLoading = false;
        }
        protected abstract void OnLevelZoneChange_IMPL();
        #endregion

        #region Logical Conditions
        public bool IsNewZoneLoading() { return isNewZoneLoading; }
        #endregion
    }

}
