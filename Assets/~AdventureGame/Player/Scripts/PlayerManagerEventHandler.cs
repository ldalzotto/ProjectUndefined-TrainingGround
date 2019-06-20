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

        public IEnumerator OnSetDestinationCoRoutine(Vector3 destination, float normalizedSpeed)
        {
            if(this.PlayerManager != null)
            {
                return PlayerManager.SetAIDestinationCoRoutine(destination, normalizedSpeed);
            }
            return null;
        }

    }

}