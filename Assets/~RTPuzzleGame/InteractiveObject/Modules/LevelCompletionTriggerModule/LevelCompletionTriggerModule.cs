using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LevelCompletionTriggerModule : InteractiveObjectModule, RangeTypeObjectEventListener
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
            this.RangeTypeObject.Init(null, eventListenersFromExterior: new List<RangeTypeObjectEventListener>() { this });
        }

        public void OnInteractiveObjectDestroyed()
        {
            this.RangeTypeObject.OnRangeDestroyed();
        }

        public void OnRangeTriggerEnter(Collider other)
        {
            Debug.Log(MyLog.Format("LevelCompletionTriggerModule OnTriggerEnter"));
            AskForLevelCompletionCalculation(other);
        }

        public void OnRangeTriggerExit(Collider other)
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
