using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {

        #region AI Managers
        private AbstractAIPatrolComponentManager aIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager aIProjectileEscapeWithCollisionManager;
        private AbstractAIProjectileEscapeManager aIProjectileIgnoringTargetZoneEscapeManager;
        private AbstractAITargetZoneManager aITargetZoneComponentManager;
        private AIFearStunManager aIFearStunComponentManager;
        private AbstractAIAttractiveObjectManager aIAttractiveObjectManager;
        #endregion

        public AbstractAIPatrolComponentManager AIPatrolComponentManager { get => aIPatrolComponentManager; }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeWithCollisionManager { get => aIProjectileEscapeWithCollisionManager; }
        public AbstractAIProjectileEscapeManager AIProjectileIgnoringTargetZoneEscapeManager { get => aIProjectileIgnoringTargetZoneEscapeManager; }
        public AbstractAITargetZoneManager AITargetZoneComponentManager { get => aITargetZoneComponentManager; }
        public AIFearStunManager AIFearStunComponentManager { get => aIFearStunComponentManager; }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager { get => aIAttractiveObjectManager; }

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, TargetZoneContainer TargetZoneContainer, AiID aiID, Collider aiCollider) : base(selfAgent, aIComponents, new GenericPuzzleAIBehaviorExternalEventManager(), OnFOVChange, ForceUpdateAIBehavior)
        {

            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { this.aIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, aIFOVManager); return null; };
            Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerSetterOperation = () => { this.aIProjectileEscapeWithCollisionManager = new AIProjectileWithCollisionEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID, TargetZoneContainer.GetTargetZonesTriggerColliders); return null; };
            Func<AIProjectileIgnorePhysicsEscapeManager> AIProjectileEscapeWithoutCollisionManagerSetterOperation = () => { this.aIProjectileIgnoringTargetZoneEscapeManager = new AIProjectileIgnorePhysicsEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID); return null; };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { this.aIFearStunComponentManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.aIFOVManager, aiID); return null; };
            Func<AIAttractiveObjectManager> AIAttractiveObjectSetterOperation = () => { this.aIAttractiveObjectManager = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); return null; };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { this.aITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.aIFOVManager, aiID); return null; };

            Action<Type> ForAllAIManagerTypesOperation = (Type managerType) => AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType, aiPatrolComponentManagerSetterOperation,
                AIProjectileEscapeWithCollisionManagerSetterOperation, AIProjectileEscapeWithoutCollisionManagerSetterOperation, AIFearStunManagerSetterOperation,
                AIAttractiveObjectSetterOperation, AITargetZoneManagerSetterOperation);

            ForAllAIManagerTypesOperation.Invoke(aIComponents.AITargetZoneComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIAttractiveObjectComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIFearStunComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeWithCollisionComponent.SelectedManagerType);
            ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeWithoutCollisionComponent.SelectedManagerType);
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
            return this.aIFOVManager.GetFOV();
        }
        #endregion

        public override Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            Vector3? newDirection = null;
            newDirection = aIFearStunComponentManager.TickComponent(d, timeAttenuationFactor);

            if (this.IsFeared())
            {
                Debug.Log(MyLog.Format("Feared"));
            }
            else
            {
                if (this.IsEscapingFromProjectileIngnoringTargetZones())
                {
                    newDirection = aIProjectileIgnoringTargetZoneEscapeManager.TickComponent();
                }
                else
                {
                    if (this.IsEscapingFromExitZone())
                    {
                        newDirection = aITargetZoneComponentManager.TickComponent();
                    }
                    else
                    {
                        if (this.IsEscapingFromProjectile())
                        {
                            newDirection = aIProjectileEscapeWithCollisionManager.TickComponent();
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
            aIProjectileEscapeWithCollisionManager.GizmoTick();
            aIFOVManager.GizmoTick();
        }

        public override void OnAIFearedStunned()
        {
            this.ComponentsStateReset(true, true, true, true, true);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    this.ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(collider.transform.position, AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerEnterAIBehaviorEvent(TargetZone.FromCollisionType(collisionType)));
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    this.ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(collider.transform.position, AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerStayAIBehaviorEvent(TargetZone.FromCollisionType(collisionType)));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent());
                }
            }
        }

        public void ComponentsStateReset(bool randomPatrolReset, bool projectileEscapeWithCollistionReset, bool projectileEscapeWithoutCollistionReset, bool attractiveObjectReset, bool targetZoneReset)
        {
            if (randomPatrolReset)
            {
                aIPatrolComponentManager.OnStateReset();
            }

            if (projectileEscapeWithCollistionReset)
            {
                aIProjectileEscapeWithCollisionManager.OnStateReset();
            }

            if (projectileEscapeWithoutCollistionReset)
            {
                aIProjectileIgnoringTargetZoneEscapeManager.OnStateReset();
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

        private void OnDestinationReached(bool randomPatrolReset, bool projectileEscapeReset, bool projectileEscapeWithoutCollistionReset, bool attractiveObjectReset, bool targetZoneReset)
        {
            if (randomPatrolReset)
            {
                aIPatrolComponentManager.OnDestinationReached();
            }

            if (projectileEscapeReset)
            {
                aIProjectileEscapeWithCollisionManager.OnDestinationReached();
            }

            if (projectileEscapeWithoutCollistionReset)
            {
                aIProjectileIgnoringTargetZoneEscapeManager.OnDestinationReached();
            }

            if (attractiveObjectReset)
            {
                aIAttractiveObjectManager.OnDestinationReached();
            }

            if (targetZoneReset)
            {
                aITargetZoneComponentManager.OnDestinationReached();
            }

            if (!this.IsFeared())
            {
                if (!this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsEscapingFromExitZone() && !this.IsEscapingFromProjectile())
                {
                    this.aIFOVManager.ResetFOV();
                }
            }

        }

        public override void OnDestinationReached()
        {
            bool resetAttractiveObjectComponent = !this.aIAttractiveObjectManager.HasSensedThePresenceOfAnAttractiveObject(); //reset only if attractive object
            this.OnDestinationReached(true, true, true, resetAttractiveObjectComponent, true);
        }

        #region State Retrieval
        public bool IsPatrolling() { return aIPatrolComponentManager.IsPatrolling(); }
        public bool IsFeared() { return aIFearStunComponentManager.IsFeared; }
        public bool IsEscapingFromProjectile() { return aIProjectileEscapeWithCollisionManager.IsEscaping(); }
        public bool IsEscapingFromProjectileIngnoringTargetZones() { return aIProjectileIgnoringTargetZoneEscapeManager.IsEscaping(); }
        public bool IsEscapingFromExitZone() { return aITargetZoneComponentManager.IsEscapingFromTargetZone; }
        public bool IsInfluencedByAttractiveObject() { return aIAttractiveObjectManager.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("StunFeared : " + aIFearStunComponentManager.IsFeared);
            GUILayout.Label("ProjEscapingWithoutTarget : " + aIProjectileEscapeWithCollisionManager.IsEscaping());
            GUILayout.Label("EscapingFromTargetZone : " + aITargetZoneComponentManager.IsEscapingFromTargetZone);
            GUILayout.Label("EscapingFromProjectile : " + aIProjectileEscapeWithCollisionManager.IsEscaping());
            GUILayout.Label("Attracted : " + aIAttractiveObjectManager.IsInfluencedByAttractiveObject());
            GUILayout.Label("Patrolling : " + aIPatrolComponentManager.IsPatrolling());
        }

        public override string ToString()
        {
            return String.Format("[StunFeared : {0}, ProjEscapingWithoutTarget : {1}, EscapingFromTargetZone : {2}, EscapingFromProjectile : {3}, Attracted : {4}, Patrolling : {5}]",
                new string[] { aIFearStunComponentManager.IsFeared.ToString(), aIProjectileEscapeWithCollisionManager.IsEscaping().ToString(),
                    aITargetZoneComponentManager.IsEscapingFromTargetZone.ToString(), aIProjectileEscapeWithCollisionManager.IsEscaping().ToString(),
                aIAttractiveObjectManager.IsInfluencedByAttractiveObject().ToString(), aIPatrolComponentManager.IsPatrolling().ToString() });
        }
    }

}
