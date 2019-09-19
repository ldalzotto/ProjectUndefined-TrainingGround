using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AIManagerContainer : MonoBehaviour
    {
        private Dictionary<AIObjectID, AIObjectType> npcAiManagers = new Dictionary<AIObjectID, AIObjectType>();

        #region External Events
        public void OnNPCAiManagerCreated(AIObjectType NPCAIManager)
        {
            this.npcAiManagers.Add(NPCAIManager.AiID, NPCAIManager);
        }
        #endregion

        #region Data Retrieval
        public AIObjectType GetNPCAiManager(AIObjectID aiID)
        {
            return npcAiManagers[aiID];
        }
        public Dictionary<AIObjectID, AIObjectType> GetNPCAiManagers()
        {
            return this.npcAiManagers;
        }
        #endregion

        public void Init()
        {
            var initialNPCAIManagers = GameObject.FindObjectsOfType<AIObjectType>();
            foreach (var initialNPCManager in initialNPCAIManagers)
            {
                initialNPCManager.Init();
            }
        }

        public void TickAlways(float d, float timeAttenuationFactor)
        {

        }

        public void TickWhenTimeFlows(float d, float timeAttenuationFactor)
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.TickWhenTimeFlows(d, timeAttenuationFactor);
            }
        }

        public void EndOfFixedTick()
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.EndOfFixedTick();
            }
        }

        public void DisableAgents()
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.DisableAgent();
            }
        }

        public void EnableAgents()
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.EnableAgent();
            }
        }

        public void GizmoTick()
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.GizmoTick();
            }
        }

        public void GUITick()
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.GUITick();
            }
        }
    }

}
