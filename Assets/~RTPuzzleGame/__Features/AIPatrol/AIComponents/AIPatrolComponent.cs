using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;
using static AIMovementDefinitions;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPatrolComponent", order = 1)]
    public class AIPatrolComponent : AbstractAIComponent
    {
        [CustomEnum()]
        public AIPatrolManagerType AIPatrolManagerType;

        [CustomEnum(ConfigurationType = typeof(AIPatrolGraphConfiguration), OpenToConfiguration = true)]
        public AIPatrolGraphID AIPatrolGraphID;
        public AIMovementSpeedDefinition AISpeed;

        public float MaxDistance;

        public override InterfaceAIManager BuildManager()
        {
            if (this.AIPatrolManagerType == AIPatrolManagerType.RANDOM)
            {
                return new AIRandomPatrolComponentMananger(this);
            }
            else
            {
                return new AIScriptedPatrolComponentManager(this);
            }
        }
    }

    public enum AIPatrolManagerType
    {
        RANDOM = 0,
        SCRIPTED = 1
    }

    public abstract class AbstractAIPatrolComponentManager : AbstractAIManager<AIPatrolComponent>, InterfaceAIManager
    {
        #region External Dependencies
        protected NavMeshAgent patrollingAgent;
        protected IFovManagerCalcuation FovManagerCalculation;
        #endregion

        protected AIObjectID aiID;

        protected AbstractAIPatrolComponentManager(AIPatrolComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }
        
        public virtual void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.aiID = AIBheaviorBuildInputData.aiID;
            this.patrollingAgent = AIBheaviorBuildInputData.selfAgent;
            this.FovManagerCalculation = AIBheaviorBuildInputData.FovManagerCalcuation;
        }

        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
        protected abstract bool IsPatrolling();
        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        public abstract void GizmoTick();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.IsPatrolling();
        }

    }

}