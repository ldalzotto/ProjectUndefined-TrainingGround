using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIAttractiveObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIAttractiveObjectComponent", order = 1)]
    public class AIAttractiveObjectComponent : AbstractAIComponent
    {
        [CustomEnum()]
        public AttractiveObjectStrategyType AttractiveObjectStrategyType;
    }

    public enum AttractiveObjectStrategyType
    {
        LOOSE = 0, PERSISTANT = 1
    }

    public abstract class AbstractAIAttractiveObjectManager : AbstractAIManager<AIAttractiveObjectComponent>, InterfaceAIManager
    {
        protected NavMeshAgent selfAgent;
        protected AIObjectID aiID;

        private AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter;

        #region State
        protected bool isAttracted;
        protected Vector3? attractionPosition;
        #endregion

        #region External Events
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        protected AttractiveObjectModule involvedAttractiveObject;

        protected AbstractAIAttractiveObjectManager(AIAttractiveObjectComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        protected void BaseInit(NavMeshAgent selfAgent, AIObjectID aiID, PuzzleEventsManager PuzzleEventsManager, AIObjectTypeSpeedSetter aIObjectTypeSpeedSetter)
        {
            this.selfAgent = selfAgent;
            this.aiID = aiID;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.AIObjectTypeSpeedSetter = aIObjectTypeSpeedSetter;
        }

        #region External Events
        public abstract void ComponentTriggerEnter(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType);
        public abstract void ComponentTriggerStay(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType);
        public abstract void ComponentTriggerExit(AttractiveObjectModule attractiveObjectType);

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

        public void OnAttractiveObjectDestroyed(AttractiveObjectModule attractiveObjectToDestroy)
        {
            if (this.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                this.OnStateReset();
            }
        }
        #endregion

        protected void SetAttractedObject(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
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

        public virtual void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            if (isAttracted)
            {
                this.AIObjectTypeSpeedSetter.SetSpeedAttenuationFactor(AIMovementDefinitions.AIMovementSpeedDefinition.RUN);
                NPCAIDestinationContext.TargetPosition = attractionPosition;
            }
        }

        #region Logical Conditions
        public bool IsManagerEnabled()
        {
            return this.IsInfluencedByAttractiveObject();
        }

        private bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectModule attractiveObjectToDestroy)
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
