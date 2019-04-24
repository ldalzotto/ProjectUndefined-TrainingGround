using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {
        #region AI Managers V2
        private AIBehaviorManagerContainer AIBehaviorManagerContainer;
        #endregion

        #region AI Manager data retrieval
        public AbstractAIPatrolComponentManager AIPatrolComponentManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAIPatrolComponentManager>(this.aIComponents.AIRandomPatrolComponent);
        }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeWithCollisionManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAIProjectileEscapeManager>(this.aIComponents.AIProjectileEscapeWithCollisionComponent);
        }
        public AbstractAIProjectileEscapeManager AIProjectileIgnoringTargetZoneEscapeManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAIProjectileEscapeManager>(this.aIComponents.AIProjectileEscapeWithoutCollisionComponent);
        }
        public AbstractAITargetZoneManager AITargetZoneManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAITargetZoneManager>(this.aIComponents.AITargetZoneComponent);
        }
        public AbstractAIFearStunManager AIFearStunManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAIFearStunManager>(this.aIComponents.AIFearStunComponent);
        }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager()
        {
            return this.AIBehaviorManagerContainer.GetManager<AbstractAIAttractiveObjectManager>(this.aIComponents.AIAttractiveObjectComponent);
        }
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, TargetZoneContainer TargetZoneContainer, AiID aiID, Collider aiCollider) : base(selfAgent, aIComponents, new GenericPuzzleAIBehaviorExternalEventManager(), OnFOVChange, ForceUpdateAIBehavior)
        {
            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { return new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, aIFOVManager); };
            Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerSetterOperation = () => { return new AIProjectileWithCollisionEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID, TargetZoneContainer.GetTargetZonesTriggerColliders); };
            Func<AIProjectileIgnorePhysicsEscapeManager> AIProjectileEscapeWithoutCollisionManagerSetterOperation = () => { return new AIProjectileIgnorePhysicsEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID); };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { return new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.aIFOVManager, aiID); };
            Func<AIAttractiveObjectManager> AIAttractiveObjectSetterOperation = () => { return new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { return new AITargetZoneManager(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.aIFOVManager, aiID); };

            Func<Type, InterfaceAIManager> ForAllAIManagerTypesOperation = (Type managerType) => AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType, aiPatrolComponentManagerSetterOperation,
                AIProjectileEscapeWithCollisionManagerSetterOperation, AIProjectileEscapeWithoutCollisionManagerSetterOperation, AIFearStunManagerSetterOperation,
                AIAttractiveObjectSetterOperation, AITargetZoneManagerSetterOperation);

            AIBehaviorManagerContainer = new AIBehaviorManagerContainer(new System.Collections.Generic.Dictionary<int, InterfaceAIManager>() {
            { 1, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIFearStunComponent.SelectedManagerType)},
            { 2, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeWithoutCollisionComponent.SelectedManagerType)},
            { 3, ForAllAIManagerTypesOperation.Invoke(aIComponents.AITargetZoneComponent.SelectedManagerType)},
            { 4,ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeWithCollisionComponent.SelectedManagerType) },
            { 5, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIAttractiveObjectComponent.SelectedManagerType) },
            { 6, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIRandomPatrolComponent.SelectedManagerType) }
            });

        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            this.ReceiveEvent(new AttractiveObectDestroyedAIBehaviorEvent(attractiveObjectToDestroy));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(collider.transform.position, AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
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

        public override void OnDestinationReached()
        {
            foreach (var aiManager in this.AIBehaviorManagerContainer.GetAllAIManagers())
            {
                aiManager.OnDestinationReached();
            }

            if (!this.IsFeared())
            {
                if (!this.IsEscapingFromProjectileIngnoringTargetZones() && !this.IsEscapingFromExitZone() && !this.IsEscapingFromProjectileWithTargetZones())
                {
                    this.aIFOVManager.ResetFOV();
                }
            }
            this.puzzleAIBehaviorExternalEventManager.AfterDestinationReached(this);
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
            this.AIFearStunManager().BeforeManagersUpdate(d, timeAttenuationFactor);

            if (this.AIFearStunManager().IsManagerEnabled())
            {
                newDirection = this.AIFearStunManager().OnManagerTick(d, timeAttenuationFactor);
            }
            else
            {
                if (this.AIProjectileIgnoringTargetZoneEscapeManager().IsManagerEnabled())
                {
                    newDirection = this.AIProjectileIgnoringTargetZoneEscapeManager().OnManagerTick(d, timeAttenuationFactor);
                }
                else
                {
                    if (this.AITargetZoneManager().IsManagerEnabled())
                    {
                        newDirection = this.AITargetZoneManager().OnManagerTick(d, timeAttenuationFactor);
                    }
                    else
                    {
                        if (this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled())
                        {
                            newDirection = this.AIProjectileEscapeWithCollisionManager().OnManagerTick(d, timeAttenuationFactor);
                        }
                        else
                        {
                            if (this.AIAttractiveObjectManager().IsManagerEnabled())
                            {
                                newDirection = this.AIAttractiveObjectManager().OnManagerTick(d, timeAttenuationFactor);
                            }
                            else
                            {
                                newDirection = this.AIPatrolComponentManager().OnManagerTick(d, timeAttenuationFactor);
                            }
                        }
                    }
                }
            }

            return newDirection;
        }

        public override void TickGizmo()
        {
            this.AIPatrolComponentManager().GizmoTick();
            this.AIProjectileEscapeWithCollisionManager().GizmoTick();
            aIFOVManager.GizmoTick();
        }

        public void ComponentsStateReset()
        {
            foreach (var aiManager in this.AIBehaviorManagerContainer.GetAllAIManagers())
            {
                aiManager.OnStateReset();
            }
        }

        #region State Retrieval
        public bool IsPatrolling() { return this.AIPatrolComponentManager().IsManagerEnabled(); }
        public bool IsFeared() { return this.AIFearStunManager().IsManagerEnabled(); }
        public bool IsEscapingFromProjectileWithTargetZones() { return this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled(); }
        public bool IsEscapingFromProjectileIngnoringTargetZones() { return this.AIProjectileIgnoringTargetZoneEscapeManager().IsManagerEnabled(); }
        public bool IsEscapingFromExitZone() { return this.AITargetZoneManager().IsManagerEnabled(); }
        public bool IsInfluencedByAttractiveObject() { return this.AIAttractiveObjectManager().IsManagerEnabled(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("StunFeared : " + this.AIFearStunManager().IsManagerEnabled());
            GUILayout.Label("ProjEscapingWithoutTarget : " + this.AIProjectileIgnoringTargetZoneEscapeManager().IsManagerEnabled());
            GUILayout.Label("EscapingFromTargetZone : " + this.AITargetZoneManager().IsManagerEnabled());
            GUILayout.Label("ProjEscapingWithTarget : " + this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled());
            GUILayout.Label("Attracted : " + this.AIAttractiveObjectManager().IsManagerEnabled());
            GUILayout.Label("Patrolling : " + this.AIPatrolComponentManager().IsManagerEnabled());
        }

        public override string ToString()
        {
            return String.Format("[StunFeared : {0}, ProjEscapingWithoutTarget : {1}, EscapingFromTargetZone : {2}, ProjEscapingWithTarget : {3}, Attracted : {4}, Patrolling : {5}]",
                new string[] { this.AIFearStunManager().IsManagerEnabled().ToString(), this.AIProjectileIgnoringTargetZoneEscapeManager().IsManagerEnabled().ToString(),
            this.AITargetZoneManager().IsManagerEnabled().ToString(), this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled().ToString(),
        this.AIAttractiveObjectManager().IsManagerEnabled().ToString(), this.AIPatrolComponentManager().IsManagerEnabled().ToString() });
        }
    }

}
