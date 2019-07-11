using GameConfigurationID;
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
        protected Vector3? attractionPosition;
        #endregion

        #region External Events
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        protected AttractiveObjectTypeModule involvedAttractiveObject;

        public AbstractAIAttractiveObjectManager(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager PuzzleEventsManager)
        {
            this.selfAgent = selfAgent;
            this.aiID = aiID;
            this.PuzzleEventsManager = PuzzleEventsManager;
        }

        #region External Events
        public abstract void OnTriggerEnter(Vector3 attractivePosition, AttractiveObjectTypeModule attractiveObjectType);
        public abstract void OnTriggerStay(Vector3 attractivePosition, AttractiveObjectTypeModule attractiveObjectType);
        public abstract void OnTriggerExit(AttractiveObjectTypeModule attractiveObjectType);

        public virtual void OnDestinationReached()
        {
            if (!this.HasSensedThePresenceOfAnAttractiveObject())
            {
                this.OnStateReset();
            }
        }

        public virtual void OnStateReset()
        {
            this.SetIsAttracted(false);
            this.involvedAttractiveObject = null;
        }

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }

        public void OnAttractiveObjectDestroyed(AttractiveObjectTypeModule attractiveObjectToDestroy)
        {
            if (this.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                this.OnStateReset();
            }
        }
        #endregion

        protected void SetAttractedObject(Vector3 attractivePosition, AttractiveObjectTypeModule attractiveObjectType)
        {
            //We only check if the center point is not occluded by obstacles. Range intersection is already done in the physics step (that leads to this event).
            //This is not completely accurate accurate -> we check if a single point is not occluded.
            if (!attractiveObjectType.SphereRange.IsOccluded(this.selfAgent.transform.position))
            {
                this.attractionPosition = attractivePosition;
                this.involvedAttractiveObject = attractiveObjectType;
                this.SetIsAttracted(true);
            }
        }

        protected void SetIsAttracted(bool value)
        {
            if (this.isAttracted && !value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Attracted_End(this.involvedAttractiveObject, this.aiID);
            }
            else if (!this.isAttracted && value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Attracted_Start(this.involvedAttractiveObject, this.aiID);
            }
            this.isAttracted = value;
        }

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public virtual Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            if (isAttracted)
            {
                return attractionPosition;
            }
            return null;
        }

        #region Logical Conditions
        public bool IsManagerEnabled()
        {
            return this.IsInfluencedByAttractiveObject();
        }

        private bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectTypeModule attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        protected bool IsInfluencedByAttractiveObject()
        {
            return this.isAttracted || this.involvedAttractiveObject != null;
        }
        private bool HasSensedThePresenceOfAnAttractiveObject()
        {
            return (this.involvedAttractiveObject != null && !this.involvedAttractiveObject.SphereRange.IsOccluded(this.selfAgent.transform.position));
        }
        #endregion
    }

}
