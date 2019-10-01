using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [ModuleMetadata("AI", "Move towards an object.")]
    [CreateAssetMenu(fileName = "AIAttractiveObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIAttractiveObjectComponent", order = 1)]
    public class AIAttractiveObjectComponent : AbstractAIComponent
    {
        public override InterfaceAIManager BuildManager()
        {
            if (this.AttractiveObjectStrategyType == AttractiveObjectStrategyType.LOOSE)
            {
                return new AIAttractiveObjectLooseManager(this);
            }
            else
            {
                return new AIAttractiveObjectPersistantManager(this);
            }
        }

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

        private AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter;
        private AIObjectDataRetriever AIObjectDataRetriever;

        #region State
        protected bool isAttracted;
        protected Vector3? attractionPosition;
        #endregion

        #region External Events
        private IAIAttractiveObjectEventListener IAIAttractiveObjectEventListener;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        protected AttractiveObjectModule involvedAttractiveObject;

        protected AbstractAIAttractiveObjectManager(AIAttractiveObjectComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.selfAgent = AIBheaviorBuildInputData.selfAgent;
            this.PuzzleEventsManager = AIBheaviorBuildInputData.PuzzleEventsManager;
            this.IAIAttractiveObjectEventListener = this.PuzzleEventsManager;
            this.AIObjectTypeSpeedSetter = AIBheaviorBuildInputData.AIObjectTypeSpeedSetter();
            this.AIObjectDataRetriever = AIBheaviorBuildInputData.AIObjectDataRetriever();
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
            this.attractionPosition = attractivePosition;
            this.involvedAttractiveObject = attractiveObjectType;
            this.SetIsAttracted(true);
        }

        protected void SetIsAttracted(bool value)
        {
            if (this.isAttracted && !value)
            {
                this.IAIAttractiveObjectEventListener.AI_AttractedObject_End(this.involvedAttractiveObject, this.AIObjectDataRetriever);
            }
            else if (!this.isAttracted && value)
            {
                this.IAIAttractiveObjectEventListener.AI_AttractedObject_Start(this.involvedAttractiveObject, this.AIObjectDataRetriever);
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
            return (this.involvedAttractiveObject != null);
        }
        #endregion
    }

}
