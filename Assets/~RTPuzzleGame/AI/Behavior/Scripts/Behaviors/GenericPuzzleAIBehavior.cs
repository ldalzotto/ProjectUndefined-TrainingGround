using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {

        #region AI Managers
        private AbstractAIPatrolComponentManager aIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager aIProjectileEscapeManager;
        private AbstractAITargetZoneManager aITargetZoneComponentManager;
        private AIFearStunManager aIFearStunComponentManager;
        private AbstractAIAttractiveObjectManager aIAttractiveObjectManager;
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, TargetZoneContainer TargetZoneContainer, AiID aiID) : base(selfAgent, aIComponents, OnFOVChange)
        {

            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { this.aIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, AIFOVManager); return null; };
            Func<AIProjectileEscapeManager> AIProjectileEscapeManagerSetterOperation = () => { this.aIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, AIFOVManager, PuzzleEventsManager, aiID, TargetZoneContainer.GetTargetZonesTriggerColliders); return null; };
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
            Vector3? newDirection = null;

            if (this.IsFeared())
            {
                aIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor, this.AIFOVManager);
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
                    if (this.IsEscapingFromProjectile())
                    {
                        newDirection = aIProjectileEscapeManager.TickComponent();
                    }
                    else
                    {
                        if (this.IsEscapingFromExitZone())
                        {
                            newDirection = aITargetZoneComponentManager.TickComponent();
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
            this.ComponentsStateReset(true, false, true, true); //no reset of projectile fov FOV intersection
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
                        this.ComponentsStateReset(true, true, true, true);
                        aIAttractiveObjectManager.OnTriggerEnter(collider, collisionType);
                    }
                }
                else if (collisionType.IsTargetZone)
                {
                    if (!IsEscapingFromProjectileIngnoringTargetZones())
                    {
                        var targetZone = TargetZone.FromCollisionType(collisionType);
                        if (targetZone != null)
                        {
                            this.ComponentsStateReset(true, true, true, true);
                            this.AIFOVManager.ResetFOV();
                            this.aITargetZoneComponentManager.TriggerTargetZoneEscape(targetZone);
                        }
                    }
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (!this.IsEscapingFromProjectile() && !this.IsEscapingFromExitZone() && !this.IsFeared())
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

        private void ComponentsStateReset(bool randomPatrolReset, bool projectileEscapeReset, bool attractiveObjectReset, bool targetZoneReset)
        {
            if (randomPatrolReset)
            {
                aIPatrolComponentManager.OnStateReset();
            }

            if (projectileEscapeReset)
            {
                aIProjectileEscapeManager.OnStateReset();
            }

            if (attractiveObjectReset)
            {
                aIAttractiveObjectManager.OnStateReset();
            }

            if (targetZoneReset)
            {
                aITargetZoneComponentManager.OnStateReset();
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
        public bool IsEscapingFromProjectile() { return aIProjectileEscapeManager.IsEscaping(); }
        public bool IsEscapingFromProjectileIngnoringTargetZones() { return aIProjectileEscapeManager.IsEscapingToFarest(); }
        public bool IsEscapingFromExitZone() { return aITargetZoneComponentManager.IsEscapingFromTargetZone; }
        public bool IsInfluencedByAttractiveObject() { return aIAttractiveObjectManager.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + aIPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscapingFromProjectile : " + aIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsEscapingFromTargetZone : " + aITargetZoneComponentManager.IsEscapingFromTargetZone);
            GUILayout.Label("IsStunFeared : " + aIFearStunComponentManager.IsFeared);
            GUILayout.Label("IsAttracted : " + aIAttractiveObjectManager.IsInfluencedByAttractiveObject());
        }
    }

}
