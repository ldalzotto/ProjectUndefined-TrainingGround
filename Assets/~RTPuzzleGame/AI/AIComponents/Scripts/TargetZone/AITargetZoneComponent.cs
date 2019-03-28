using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public TargetZoneID TargetZoneID;
        public float TargetZoneEscapeDistance;

        protected override Type abstractManagerType => typeof(AbstractAITargetZoneManager);
    }

    public class AITargetZoneComponentManagerDataRetrieval
    {
        private AITargetZoneManager AITargetZoneComponentManagerRef;

        public AITargetZoneComponentManagerDataRetrieval(AITargetZoneManager aITargetZoneComponentManagerRef)
        {
            AITargetZoneComponentManagerRef = aITargetZoneComponentManagerRef;
        }

        #region Data Retrieval
        public TargetZone GetTargetZone()
        {
            return this.AITargetZoneComponentManagerRef.GetTargetZone();
        }

        public TargetZoneInherentData GetTargetZoneConfigurationData()
        {
            return this.AITargetZoneComponentManagerRef.TargetZoneConfigurationData;
        }
        #endregion

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return this.AITargetZoneComponentManagerRef.IsInTargetZone();
        }
        #endregion
    }


    public abstract class AbstractAITargetZoneManager
    {
        #region State
        protected bool isInTargetZone;
        protected bool isEscapingFromTargetZone;
        #endregion

        protected TargetZone targetZone;

        protected Vector3? escapeDestination;

        public bool IsEscapingFromTargetZone { get => isEscapingFromTargetZone; }
        public Vector3? GetCurrentEscapeDestination()
        {
            return this.escapeDestination;
        }

        public void ClearEscapeDestination()
        {
            this.escapeDestination = null;
        }

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return isInTargetZone;
        }
        #endregion

        #region Data Retrieval
        public TargetZone GetTargetZone()
        {
            return this.targetZone;
        }
        #endregion

        //TODO -> Delete this retrieval
        #region Internal dependencies
        protected AITargetZoneComponentManagerDataRetrieval aITargetZoneComponentManagerDataRetrieval;
        public AITargetZoneComponentManagerDataRetrieval AITargetZoneComponentManagerDataRetrieval { get => aITargetZoneComponentManagerDataRetrieval; }
        #endregion

        public abstract void TickComponent();
        public abstract Nullable<Vector3> TriggerTargetZoneEscape();
        public abstract void OnDestinationReached();
    }

}
