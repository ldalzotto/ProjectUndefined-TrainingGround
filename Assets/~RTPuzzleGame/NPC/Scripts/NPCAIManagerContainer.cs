using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class NPCAIManagerContainer : MonoBehaviour
    {
        private Dictionary<AiID, NPCAIManager> npcAiManagers = new Dictionary<AiID, NPCAIManager>();

        #region External Events
        public void OnNPCAiManagerCreated(NPCAIManager NPCAIManager)
        {
            this.npcAiManagers.Add(NPCAIManager.AiID, NPCAIManager);
        }
        public void OnGameOver()
        {
            foreach(var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.OnGameOver();
            }
        }
        #endregion

        #region Data Retrieval
        public NPCAIManager GetNPCAiManager(AiID aiID)
        {
            return npcAiManagers[aiID];
        }
        #endregion
    }

}
