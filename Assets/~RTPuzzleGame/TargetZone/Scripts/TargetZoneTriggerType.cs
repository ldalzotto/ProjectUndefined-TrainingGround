using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneTriggerType : MonoBehaviour
    {
        #region External dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private Collider targetZoneTriggerCollider;

        public Collider TargetZoneTriggerCollider { get => targetZoneTriggerCollider; }

        public void Init()
        {
            this.targetZoneTriggerCollider = GetComponent<Collider>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            GetComponent<BoxRangeType>().Init();
        }

        private void OnTriggerEnter(Collider other)
        {
            AskForLevelCompletionCalculation(other);
        }

        private void OnTriggerExit(Collider other)
        {
            AskForLevelCompletionCalculation(other);
        }

        private void AskForLevelCompletionCalculation(Collider other)
        {
            var npcAiManager = NPCAIManager.FromCollisionType(other.GetComponent<CollisionType>());
            if (npcAiManager != null)
            {
                this.PuzzleEventsManager.PZ_EVT_LevelCompletion_ConditionRecalculationEvaluate();
            }
        }
    }

}
