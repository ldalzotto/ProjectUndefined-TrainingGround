using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneTriggerType : MonoBehaviour
    {
        #region External dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private RangeTypeObject RangeTypeObject;

        public Collider GetTargetZoneTriggerCollider()
        {
            return this.RangeTypeObject.RangeType.GetCollider();
        }

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.RangeTypeObject = GetComponentInChildren<RangeTypeObject>();
            this.RangeTypeObject.Init(null);
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
