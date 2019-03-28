using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public TargetZoneID TargetZoneID;

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
            return this.AITargetZoneComponentManagerRef.TargetZone;
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
        #endregion

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return isInTargetZone;
        }
        #endregion

        //TODO -> Delete this retrieval
        #region Internal dependencies
        protected AITargetZoneComponentManagerDataRetrieval aITargetZoneComponentManagerDataRetrieval;
        public AITargetZoneComponentManagerDataRetrieval AITargetZoneComponentManagerDataRetrieval { get => aITargetZoneComponentManagerDataRetrieval; }
        #endregion

        public abstract void TickComponent();
    }

}
