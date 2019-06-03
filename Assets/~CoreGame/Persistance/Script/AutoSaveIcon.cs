using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CoreGame
{
    public class AutoSaveIcon : MonoBehaviour
    {
        #region Internal Dependencies
        private Image autoSaveIcon;
        #endregion

        public void Init()
        {
            this.autoSaveIcon = GetComponent<Image>();
            this.autoSaveIcon.enabled = false;
        }

        #region External events
        public void OnSaveStart()
        {
            this.autoSaveIcon.enabled = true;
        }

        public void OnSaveEnd()
        {
            this.autoSaveIcon.enabled = false;
        }
        #endregion
    }
}
