using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIAttractiveObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIAttractiveObjectComponent", order = 1)]
    public class AIAttractiveObjectComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIAttractiveObjectManager);
    }



    public abstract class AbstractAIAttractiveObjectManager : InterfaceAIManager
    {

        protected NavMeshAgent selfAgent;
        protected AiID aiID;

        #region State
        protected bool isAttracted;
        #endregion
        protected AttractiveObjectType involvedAttractiveObject;
        
        #region External Events
        public abstract void OnTriggerEnter(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType);
        public abstract void OnTriggerStay(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType);
        public abstract void OnTriggerExit();
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }

        public void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            if (this.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                this.OnStateReset();
            }
        }
        #endregion

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        #region Logical Conditions
        public bool IsManagerEnabled()
        {
            return this.IsInfluencedByAttractiveObject();
        }

        private bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectType attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        protected bool IsInfluencedByAttractiveObject()
        {
            return this.isAttracted || this.HasSensedThePresenceOfAnAttractiveObject();
        }
        public bool HasSensedThePresenceOfAnAttractiveObject()
        {
            return (this.involvedAttractiveObject != null && this.involvedAttractiveObject.IsInRangeOf(this.selfAgent.transform.position));
        }
        #endregion
    }

}
