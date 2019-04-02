using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {

        #region AI Components
        private AbstractAIPatrolComponentManager aIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager aIProjectileEscapeManager;
        private AbstractAITargetZoneManager aITargetZoneComponentManager;
        private AIFearStunManager aIFearStunComponentManager;
        private AbstractAIAttractiveObjectManager aIAttractiveObjectManager;
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(selfAgent, aIComponents, OnFOVChange)
        {

            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { this.aIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, AIFOVManager); return null; };
            Func<AIProjectileEscapeManager> AIProjectileEscapeManagerSetterOperation = () => { this.aIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, AIFOVManager, PuzzleEventsManager, aiID, aITargetZoneComponentManager.GetTargetZone); return null; };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { this.aIFearStunComponentManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, aiID); return null; };
            Func<AIAttractiveObjectManager> AIAttractiveObjectSetterOperation = () => { this.aIAttractiveObjectManager = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); return null; };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { this.aITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aIComponents.AITargetZoneComponent, this.AIFOVManager); return null; };

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
            aIAttractiveObjectManager.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
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
            aITargetZoneComponentManager.TickComponent();

            Vector3? newDirection = null;

            if (this.IsFeared())
            {
                aIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor);
            }
            else
            {
                var fearStunPosition = aIFearStunComponentManager.TickComponent(AIFOVManager);
                if (this.IsFeared())
                {
                    newDirection = fearStunPosition.Value; //position is the agent itself
                }
                else
                {
                    if (this.IsEscapingFromProjectileOrExitZone())
                    {
                        newDirection = aIProjectileEscapeManager.TickComponent();
                        if (newDirection == null)
                        {
                            newDirection = aITargetZoneComponentManager.GetCurrentEscapeDestination();
                        }
                    }
                    else
                    {
                        if (this.IsInTargetZone())
                        {
                            newDirection = aITargetZoneComponentManager.TriggerTargetZoneEscape();
                        }
                        else
                        {
                            if (this.IsInfluencedByAttractiveObject())
                            {
                                newDirection = aIAttractiveObjectManager.TickComponent();
                            }
                            else
                            {
                                newDirection = aIPatrolComponentManager.TickComponent();
                            }
                        }
                    }
                }
            }

            return newDirection;
        }

        public override void TickGizmo()
        {
            aIPatrolComponentManager.GizmoTick();
            aIProjectileEscapeManager.GizmoTick();
            AIFOVManager.GizmoTick();
        }

        public override void OnProjectileTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            this.OnDestinationReached(true, false, true, true); //no reset of projectile fov FOV intersection
            aIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!aIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        this.OnDestinationReached();
                        aIAttractiveObjectManager.OnTriggerEnter(collider, collisionType);
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
                    aIAttractiveObjectManager.OnTriggerStay(collider, collisionType);
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
                    if (!aIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        aIAttractiveObjectManager.OnTriggerExit(collider, collisionType);
                    }
                }
            }
        }

        private void OnDestinationReached(bool randomPatrolReset, bool projectileEscapeReset, bool attractiveObjectReset, bool targetZoneReset)
        {
            if (randomPatrolReset)
            {
                aIPatrolComponentManager.OnDestinationReached();
            }

            if (projectileEscapeReset)
            {
                aIProjectileEscapeManager.OnDestinationReached();
            }

            if (attractiveObjectReset)
            {
                aIAttractiveObjectManager.OnDestinationReached();
            }

            if (targetZoneReset)
            {
                aITargetZoneComponentManager.OnDestinationReached();
            }
        }

        public override void OnDestinationReached()
        {
            bool resetAttractiveObjectComponent = !this.aIAttractiveObjectManager.HasSensedThePresenceOfAnAttractiveObject(); //reset only if attractive object
            this.OnDestinationReached(true, true, resetAttractiveObjectComponent, true);
        }

        #region State Retrieval
        public bool IsPatrolling() { return aIPatrolComponentManager.IsPatrolling(); }
        public bool IsFeared() { return aIFearStunComponentManager.IsFeared; }
        public bool IsEscapingFromProjectileOrExitZone() { return aIProjectileEscapeManager.IsEscaping() || aITargetZoneComponentManager.IsEscapingFromTargetZone; }
        public bool IsInTargetZone() { return aITargetZoneComponentManager.IsInTargetZone(); }
        public bool IsInfluencedByAttractiveObject() { return aIAttractiveObjectManager.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + aIPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscaping : " + aIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsInTargetZone : " + aITargetZoneComponentManager.IsInTargetZone());
            GUILayout.Label("IsStunFeared : " + aIFearStunComponentManager.IsFeared);
            GUILayout.Label("IsAttracted : " + aIAttractiveObjectManager.IsInfluencedByAttractiveObject());
        }
    }

}
