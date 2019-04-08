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

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, TargetZoneContainer TargetZoneContainer, AiID aiID, Collider aiCollider) : base(selfAgent, aIComponents, OnFOVChange, ForceUpdateAIBehavior)
        {

            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { this.aIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, AIFOVManager); return null; };
            Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerSetterOperation = () => { this.aIProjectileEscapeWithCollisionManager = new AIProjectileWithCollisionEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, AIFOVManager, PuzzleEventsManager, aiID, TargetZoneContainer.GetTargetZonesTriggerColliders); return null; };
            Func<AIProjectileIgnorePhysicsEscapeManager> AIProjectileEscapeWithoutCollisionManagerSetterOperation = () => { this.aIProjectileIgnoringTargetZoneEscapeManager = new AIProjectileIgnorePhysicsEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, AIFOVManager, PuzzleEventsManager, aiID); return null; };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { this.aIFearStunComponentManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.AIFOVManager, aiID); return null; };
            Func<AIAttractiveObjectManager> AIAttractiveObjectSetterOperation = () => { this.aIAttractiveObjectManager = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); return null; };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { this.aITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.AIFOVManager, aiID); return null; };

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
            return this.AIFOVManager.GetFOV();
        }
        #endregion

        public override Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            Vector3? newDirection = null;
            newDirection = aIFearStunComponentManager.TickComponent(d, timeAttenuationFactor);

            if (this.IsFeared())
            {
                Debug.Log("Feared");
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
            AIFOVManager.GizmoTick();
        }

        public override void OnAiFearEnd()
        {
            this.ComponentsStateReset(true, true, true, true, true);
            // to not have inactive frame.
            this.ForceUpdateAIBehavior.Invoke();
        }

        public override void OnProjectileTriggerEnter(Collider collider)
        {
            if (!this.IsFeared())
            {
                var collisionType = collider.GetComponent<CollisionType>();
                //if already escape from exit zone or escaping from projectile -> escape with ignoring
                if (this.IsEscapingFromProjectile() || this.IsEscapingFromProjectileIngnoringTargetZones())
                {
                    Debug.Log(Time.frameCount + "AI - OnProjectileTriggerEnter");
                    this.ComponentsStateReset(true, true, true, true, true);
                    this.aIProjectileIgnoringTargetZoneEscapeManager.OnTriggerEnter(collider, collisionType);
                }
                else
                {
                    Debug.Log(Time.frameCount + "AI - OnProjectileTriggerEnter");
                    this.ComponentsStateReset(true, true, true, true, true);
                    this.aIProjectileEscapeWithCollisionManager.OnTriggerEnter(collider, collisionType);
                }
            }
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!this.IsEscapingFromProjectile() && !this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsEscapingFromExitZone() && !this.IsFeared())
                    {
                        Debug.Log(Time.frameCount + "AI - OnAttractiveObjectTriggerEnter");
                        this.ComponentsStateReset(true, true, true, true, true);
                        aIAttractiveObjectManager.OnTriggerEnter(collider, collisionType);
                    }
                }
                else if (collisionType.IsTargetZone)
                {
                    if (!this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsFeared())
                    {
                        var targetZone = TargetZone.FromCollisionType(collisionType);
                        if (targetZone != null)
                        {
                            if (!this.IsEscapingFromProjectile())
                            {
                                this.AIFOVManager.ResetFOV();
                            }

                            Debug.Log(Time.frameCount + "AI - OnTargetZoneTriggerEnter");
                            this.ComponentsStateReset(true, true, true, true, true);
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
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!this.IsInfluencedByAttractiveObject())
                    {
                        if (!this.IsEscapingFromProjectile() && !this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsEscapingFromExitZone() && !this.IsFeared())
                        {
                            Debug.Log(Time.frameCount + "AI - OnAttractiveObjectTriggerStay");
                            this.ComponentsStateReset(true, true, true, true, true);
                            aIAttractiveObjectManager.OnTriggerStay(collider, collisionType);
                        }
                    }

                }
                else if (collisionType.IsTargetZone)
                {
                    if (!this.IsEscapingFromExitZone())
                    {
                        if (!this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsFeared())
                        {
                            if (!this.IsEscapingFromProjectile())
                            {
                                this.AIFOVManager.ResetFOV();
                            }
                            var targetZone = TargetZone.FromCollisionType(collisionType);
                            if (targetZone != null)
                            {
                                Debug.Log(Time.frameCount + "AI - OnTargetZoneTriggerStay");
                                this.ComponentsStateReset(true, true, true, true, true);
                                this.aITargetZoneComponentManager.TriggerTargetZoneEscape(targetZone);
                            }
                        }
                    }
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
                    if (!aIProjectileEscapeWithCollisionManager.IsEscaping() && !this.IsFeared())
                    {
                        aIAttractiveObjectManager.OnTriggerExit(collider, collisionType);
                    }
                }
            }
        }

        private void ComponentsStateReset(bool randomPatrolReset, bool projectileEscapeWithCollistionReset, bool projectileEscapeWithoutCollistionReset, bool attractiveObjectReset, bool targetZoneReset)
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
                    this.AIFOVManager.ResetFOV();
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
