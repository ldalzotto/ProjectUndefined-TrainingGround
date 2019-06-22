using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{

    public class PlayerManagerEventHandler : MonoBehaviour
    {

        #region External Dependencies
        private PlayerManager PlayerManager;
        #endregion

        void Start()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        }

        [Obsolete("Cutscene movement must be handled with the new system")]
        public IEnumerator OnSetDestinationCoRoutine(Transform destination, float normalizedSpeed)
        {
            if (this.PlayerManager != null)
            {
                return PlayerManager.SetAIDestinationCoRoutine(destination, normalizedSpeed);
            }
            return null;
        }

    }

}