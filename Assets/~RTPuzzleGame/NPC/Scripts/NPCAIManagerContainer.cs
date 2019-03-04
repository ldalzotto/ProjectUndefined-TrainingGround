using System;
using System.Collections.Generic;
using UnityEngine;

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
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.OnGameOver();
            }
        }
        internal void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
            }
        }
        #endregion

        #region Data Retrieval
        public NPCAIManager GetNPCAiManager(AiID aiID)
        {
            return npcAiManagers[aiID];
        }
        #endregion

        public void Init()
        {
            var initialNPCAIManagers = GameObject.FindObjectsOfType<NPCAIManager>();
            foreach(var initialNPCManager in initialNPCAIManagers)
            {
                initialNPCManager.Init();
            }
        }

        public void TickAlways(float d, float timeAttenuationFactor)
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.TickAlways(d, timeAttenuationFactor);
            }
        }

        public void TickWhenTimeFlows(float d, float timeAttenuationFactor)
        {
            foreach (var npcAiManager in npcAiManagers.Values)
            {
                npcAiManager.TickWhenTimeFlows(d, timeAttenuationFactor);
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
