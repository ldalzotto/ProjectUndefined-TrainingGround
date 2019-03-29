using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {

        #region AI Components
        private AbstractAIPatrolComponentManager AIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager AIProjectileEscapeManager;
        private AbstractAITargetZoneManager AITargetZoneComponentManager;
        private AIFearStunManager AIFearStunComponentManager;
        private AbstractAIAttractiveObjectManager AIAttractiveObjectManager;
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(selfAgent, aIComponents, OnFOVChange)
        {

            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { this.AIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, AIFOVManager); return null; };
            Func<AIProjectileEscapeManager> AIProjectileEscapeManagerSetterOperation = () => { this.AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, AIFOVManager, PuzzleEventsManager, aiID, AITargetZoneComponentManager.GetTargetZone); return null; };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { this.AIFearStunComponentManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, aiID); return null; };
            Func<AIAttractiveObjectManager> AIAttractiveObjectSetterOperation = () => { this.AIAttractiveObjectManager = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); return null; };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { this.AITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aIComponents.AITargetZoneComponent, this.AIFOVManager); return null; };

            Action<Type> ForAllAIManagerTypesOperation = (Type managerType) => AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType, aiPatrolComponentManagerSetterOperation, AIProjectileEscapeManagerSetterOperation, AIFearStunManagerSetterOperation, AIAttractiveObjectSetterOperation, AITargetZoneManagerSetterOperation);

            ForAllAIManagerTypesOperation.Invoke(aIComponents.AITargetZoneComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIAttractiveObjectComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIFearStunComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIRandomPatrolComponent.SelectedManagerType);
        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            AIAttractiveObjectManager.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }
        #endregion

        #region Logical Conditions
        private bool IsReactingToProjectile()
        {
            return !this.IsFeared();
        }
        #endregion

        #region Data Retrieval
        public FOV GetFOV()
        {
            return this.AIFOVManager.GetFOV();
        }
        #endregion

        public override Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            AITargetZoneComponentManager.TickComponent();

            Vector3? newDirection = null;

            if (this.IsFeared())
            {
                AIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor);
            }
            else
            {
                var fearStunPosition = AIFearStunComponentManager.TickComponent(AIFOVManager);
                if (this.IsFeared())
                {
                    newDirection = fearStunPosition.Value; //position is the agent itself
                }
                else
                {
                    if (this.IsEscapingFromProjectileOrExitZone())
                    {
                        newDirection = AIProjectileEscapeManager.GetCurrentEscapeDirection();
                        if (newDirection == null)
                        {
                            newDirection = AITargetZoneComponentManager.GetCurrentEscapeDestination();
                        }
                    }
                    else
                    {
                        if (this.IsInTargetZone())
                        {
                            newDirection = AITargetZoneComponentManager.TriggerTargetZoneEscape();
                        }
                        else
                        {
                            if (this.IsInfluencedByAttractiveObject())
                            {
                                newDirection = AIAttractiveObjectManager.TickComponent();
                            }
                            else
                            {
                                newDirection = AIPatrolComponentManager.TickComponent();
                            }
                        }
                    }
                }
            }

            return newDirection;
        }

        public override void TickGizmo()
        {
            AIPatrolComponentManager.GizmoTick();
            AIProjectileEscapeManager.GizmoTick();
            AIFOVManager.GizmoTick();
        }

        public override void OnProjectileTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            this.OnDestinationReached(true, false, true, true); //no reset of projectile fov FOV intersection
            AIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        this.OnDestinationReached();
                        AIAttractiveObjectManager.OnTriggerEnter(collider, collisionType);
                    }
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (!this.IsEscapingFromProjectileOrExitZone() && !this.IsFeared())
                {
                    AIAttractiveObjectManager.OnTriggerStay(collider, collisionType);
                }
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        AIAttractiveObjectManager.OnTriggerExit(collider, collisionType);
                    }
                }
            }
        }

        private void OnDestinationReached(bool randomPatrolReset, bool projectileEscapeReset, bool attractiveObjectReset, bool targetZoneReset)
        {
            if (randomPatrolReset)
            {
                AIPatrolComponentManager.OnDestinationReached();
            }

            if (projectileEscapeReset)
            {
                AIProjectileEscapeManager.OnDestinationReached();
            }

            if (attractiveObjectReset)
            {
                AIAttractiveObjectManager.OnDestinationReached();
            }

            if (targetZoneReset)
            {
                AITargetZoneComponentManager.OnDestinationReached();
            }
        }

        public override void OnDestinationReached()
        {
            bool resetAttractiveObjectComponent = !this.AIAttractiveObjectManager.HasSensedThePresenceOfAnAttractiveObject(); //reset only if attractive object
            this.OnDestinationReached(true, true, resetAttractiveObjectComponent, true);
        }

        #region State Retrieval
        public bool IsPatrolling() { return AIPatrolComponentManager.IsPatrolling(); }
        public bool IsFeared() { return AIFearStunComponentManager.IsFeared; }
        public bool IsEscapingFromProjectileOrExitZone() { return AIProjectileEscapeManager.IsEscaping() || AITargetZoneComponentManager.IsEscapingFromTargetZone; }
        public bool IsInTargetZone() { return AITargetZoneComponentManager.IsInTargetZone(); }
        public bool IsInfluencedByAttractiveObject() { return AIAttractiveObjectManager.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + AIPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsInTargetZone : " + AITargetZoneComponentManager.IsInTargetZone());
            GUILayout.Label("IsStunFeared : " + AIFearStunComponentManager.IsFeared);
            GUILayout.Label("IsAttracted : " + AIAttractiveObjectManager.IsInfluencedByAttractiveObject());
        }
    }

}
