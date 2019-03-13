using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIFearStunComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIFearStunComponent", order = 1)]
    public class AIFearStunComponent : ScriptableObject
    {
        public float FOVSumThreshold;
        public float TimeWhileBeginFeared;
    }


    public class AIFearStunComponentManager
    {
        private NavMeshAgent currentAgent;
        private AIFearStunComponent AIFearStunComponent;
        private PuzzleEventsManager PuzzleEventsManager;
        private AiID AiID;

        public AIFearStunComponentManager(NavMeshAgent currentAgent, AIFearStunComponent aIFearStunComponent, PuzzleEventsManager PuzzleEventsManager, AiID AiID)
        {
            this.currentAgent = currentAgent;
            AIFearStunComponent = aIFearStunComponent;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.AiID = AiID;
        }

        #region State
        private bool isFeared;
        #endregion

        public bool IsFeared { get => isFeared; }


        private float fearedTimer;

        public Vector3? TickComponent(AIFOVManager aIFOVManager)
        {
            if (!isFeared)
            {
                if (aIFOVManager.GetFOVAngleSum() <= this.AIFearStunComponent.FOVSumThreshold)
                {
                    this.SetIsFeared(true);
                    this.fearedTimer = 0f;
                    return this.currentAgent.transform.position;
                }
            }

            return null;
        }

        public void TickWhileFeared(float d, float timeAttenuationFactor)
        {
            if (isFeared)
            {
                fearedTimer += (d * timeAttenuationFactor);
                if(fearedTimer >= AIFearStunComponent.TimeWhileBeginFeared)
                {
                    this.SetIsFeared(false);
                }
            }
        }

        private void SetIsFeared(bool newIsFearedValue)
        {
            if (newIsFearedValue)
            {
                if (!this.isFeared)
                {
                    this.PuzzleEventsManager.OnAIFearedStunned(this.AiID);
                }
            } else
            {
                if (this.isFeared)
                {
                    this.PuzzleEventsManager.OnAIFearedStunnedEnded(this.AiID);
                }
            }
            this.isFeared = newIsFearedValue;
        }
    }
}
